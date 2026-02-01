using Api_health_check.Clients;
using Api_health_check.Data;
using Api_health_check.Models;
using Api_health_check.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;

namespace Api_health_check.Services
{
    public class UrlHealthCheckService : BackgroundService
    {
        private readonly ILogger<UrlHealthCheckService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailSender _emailSender;
      
        //private readonly RecoveryOrchestratorService _recoveryOrchestrator;

        // keep the main loop tight so we can honor per-endpoint intervals in-memory
        private readonly TimeSpan _loopDelay = TimeSpan.FromSeconds(1);

        public UrlHealthCheckService(
            ILogger<UrlHealthCheckService> logger,
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            IEmailSender emailSender
            ///* RecoveryOrchestratorService recoveryOrchestrator*/
           )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory; 
            _emailSender = emailSender;
            
            ////_recoveryOrchestrator = recoveryOrchestrator;
        }

        private bool IsInRecoveryCoolDown(MonitoredEndpoint endpoint)
        {
            if (!endpoint.LastCheckUtc.HasValue)
                return false;
            var nextAllowedRecovery = endpoint.LastRecoveryUtc.Value.AddSeconds(endpoint.RecoveryCoolDownSeconds);
            return DateTime.UtcNow < nextAllowedRecovery;   
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UrlHealthCheckService started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<HealthCheckContext>();
                    var recoveryOrchestrator = scope.ServiceProvider.GetRequiredService<RecoveryOrchestratorService>();

                    // load active endpoints and decide in-memory which ones are due
                    var endpoints = await db.MonitoredEndpoints
                        .Where(e => e.IsActive)
                        .ToListAsync(stoppingToken);

                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(10);

                    var now = DateTime.UtcNow;

                    foreach (var endpoint in endpoints)
                    {
                        // honor per-endpoint CheckIntervalSeconds
                        if (endpoint.LastCheckUtc.HasValue)
                        {
                            var nextDue = endpoint.LastCheckUtc.Value.AddSeconds(endpoint.CheckIntervalSeconds);
                            if (now < nextDue)
                            {
                                // not due yet
                                continue;
                            }
                        }

                        var wasDown = endpoint.IsDown;

                        try
                        {
                            var response = await client.GetAsync(endpoint.Url, stoppingToken);

                            endpoint.LastStatusCode = (int)response.StatusCode;
                            endpoint.IsDown = !response.IsSuccessStatusCode;
                            endpoint.ConsecutiveFailures = endpoint.IsDown
                                ? endpoint.ConsecutiveFailures + 1
                                : 0;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error checking {Url}", endpoint.Url);
                            endpoint.LastStatusCode = null;
                            endpoint.IsDown = true;
                            endpoint.ConsecutiveFailures++;
                        }

                        endpoint.LastCheckUtc = DateTime.UtcNow;

                        // Defensive: ensure EF Core will persist the core properties
                        try
                        {
                            var entry = db.Entry(endpoint);
                            if (entry.State == EntityState.Detached)
                            {
                                db.Attach(endpoint);
                                entry = db.Entry(endpoint);
                            }

                            entry.Property(e => e.LastCheckUtc).IsModified = true;
                            entry.Property(e => e.ConsecutiveFailures).IsModified = true;
                            entry.Property(e => e.IsDown).IsModified = true;
                            entry.Property(e => e.LastStatusCode).IsModified = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to mark core properties as modified for endpoint {EndpointId}", endpoint.Id);
                        }

                        // Persist core check result immediately so notification failures can't prevent it
                        try
                        {
                            var saved = await db.SaveChangesAsync(stoppingToken);
                            _logger.LogDebug("Persisted core check result for endpoint {EndpointId}, saved {Count} changes.", endpoint.Id, saved);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to persist core check result for endpoint {EndpointId}", endpoint.Id);
                            // continue to notification attempts even if save failed; we don't rethrow
                        }

                        _logger.LogInformation("Checked endpoint {Name} ({Url}): IsDown={IsDown}, LastStatus={Status}, Failures={Failures}, LastCheck={LastCheck}",
                            endpoint.Name, endpoint.Url, endpoint.IsDown, endpoint.LastStatusCode?.ToString() ?? "-", endpoint.ConsecutiveFailures, endpoint.LastCheckUtc?.ToString("u") ?? "-");

                        // 🔁 AUTO-RECOVERY (after consecutive failures)
                        if (endpoint.IsDown &&
                            endpoint.ConsecutiveFailures >= 3 &&
                            endpoint.NotifyOnRecovery) // reuse flag as enable/disable
                        {
                            if (IsInRecoveryCoolDown(endpoint))
                            {
                                _logger.LogInformation(
                                                       "Recovery skipped for endpoint {endpointId} due to cooldown time, LastRecoverAtUtc = {LastRecoveryUtc} ",
                                                       endpoint.Id,
                                                       endpoint.LastCheckUtc);

                                db.RecoveryAuditLos.Add(new Models.RecoveryAuditLog
                                {
                                    EndpointId = endpoint.Id,
                                    Action = "COOLDOWN_SKIP",
                                    Success = true,
                                    Message = "Recovery skipped due to cooldown period",
                                    AttemptedAtUtc = DateTime.UtcNow,
                                    RecoveryWebhookUrl = endpoint.RecoveryWebhookUrl
                                });
                                await db.SaveChangesAsync(stoppingToken);
                            }

                            else
                            {
                                _logger.LogWarning(
                                    "Endpoint is being triggerd for endpoint {EndpointId} after {Consecutivefailure} consecutive failure",
                                    endpoint.Id,
                                    endpoint.ConsecutiveFailures);
                                await recoveryOrchestrator.RecoverAsync(endpoint);
                                //update last recovery time
                                endpoint.LastRecoveryUtc = DateTime.UtcNow;
                                db.Entry(endpoint).Property(e => e.LastRecoveryUtc).IsModified = true;
                                await db.SaveChangesAsync(stoppingToken);
                            }
                        }

                            //try
                            //{
                            //    _logger.LogWarning(
                            //        "Endpoint {EndpointId} is DOWN for {Failures} checks. Triggering recovery.",
                            //        endpoint.Id,
                            //        endpoint.ConsecutiveFailures);

                            //    await recoveryOrchestrator.RecoverAsync(endpoint);
                            //}
                            //catch (Exception ex)
                            //{
                            //    _logger.LogError(
                            //        ex,
                            //        "Recovery attempt failed for endpoint {EndpointId}",
                            //        endpoint.Id);
                            //}
                        

                        // DOWN email: only once per outage
                        if (endpoint.IsDown &&
                            !endpoint.AlertSentForCurrentDown &&
                            !string.IsNullOrWhiteSpace(endpoint.NotifyEmail))
                        {
                            try
                            {
                                var subject = $"ALERT: {endpoint.Name} is DOWN";
                                var body =
                                    $"The endpoint {endpoint.Url} appears to be DOWN as of {DateTime.UtcNow:u}. " +
                                    $"Last status code: {(endpoint.LastStatusCode?.ToString() ?? "none (exception)")}.";

                                await _emailSender.SendAsync(endpoint.NotifyEmail, subject, body);

                                // mark and persist that we've sent the alert
                                endpoint.AlertSentForCurrentDown = true;
                                try
                                {
                                    db.Entry(endpoint).Property(e => e.AlertSentForCurrentDown).IsModified = true;
                                    await db.SaveChangesAsync(stoppingToken);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to persist AlertSentForCurrentDown for endpoint {EndpointId}", endpoint.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to send down notification for {Url}", endpoint.Url);
                                // do not rethrow — core result already persisted
                            }
                        }

                        // UP email: when it recovers
                        if (!endpoint.IsDown &&
                            endpoint.AlertSentForCurrentDown && 
                            endpoint.NotifyOnRecovery &&
                            !string.IsNullOrWhiteSpace(endpoint.NotifyEmail))
                        {
                            try
                            {
                                var subject = $"RESOLVED: {endpoint.Name} is UP again";
                                var body =
                                    $"The endpoint {endpoint.Url} is responding again as of {DateTime.UtcNow:u}. " +
                                    $"Last status code: {(endpoint.LastStatusCode?.ToString() ?? "none")}.";

                                await _emailSender.SendAsync(endpoint.NotifyEmail, subject, body);

                                endpoint.AlertSentForCurrentDown = false; // reset for next outage
                                try
                                {
                                    db.Entry(endpoint).Property(e => e.AlertSentForCurrentDown).IsModified = true;
                                    await db.SaveChangesAsync(stoppingToken);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to persist AlertSentForCurrentDown reset for endpoint {EndpointId}", endpoint.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to send recovery notification for {Url}", endpoint.Url);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in UrlHealthCheckService loop.");
                }

                await Task.Delay(_loopDelay, stoppingToken);
            }

            _logger.LogInformation("UrlHealthCheckService stopping at: {time}", DateTimeOffset.Now);
        }
    }
}
