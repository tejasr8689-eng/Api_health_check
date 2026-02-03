using Api_health_check.Data;
using Api_health_check.Models;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Api_health_check.Clients
{
    public class RecoveryClient : IRecoveryClient
    {
        private readonly HttpClient _http;
        private readonly HealthCheckContext _context;

        public RecoveryClient(HttpClient http, HealthCheckContext context)
        {
            _http = http;
            _context = context;
        }

        public static string ShortId (int Length)
        {
            return Guid.NewGuid().
                   ToString("N").Substring(0, Length).ToUpper();
        }
        public static string GenerateRequestId()
        {
            String shortId =  ShortId(10);
            return $"RCV-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Random.Shared.Next(1000, 9999)}";
        }

        public async Task<bool> TriggerRecoveryAsync(MonitoredEndpoint endpoint)
        {
            
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "X-Recovery-Key",
                    "tejasrapikeyforhealthcheckapi@8689973corm"
                );
                
                
                
                var payload = new
                {
                    requestId = GenerateRequestId(), // 👈 generated
                    action = endpoint.RecoveryAction,          // 👈 user provided
                    target = endpoint.AppPoolName,      // 👈 user provided
                    webHookUrl= endpoint.RecoveryWebhookUrl 
                };

                var recoveryRequest = new RecoveryRequest
                {
                    RequestId = payload.requestId,
                    EndpointId = endpoint.Id,
                    action = payload.action.ToString(),
                    target = payload.target,
                    requestedAt = DateTime.UtcNow,

                };
                _context.RecoveryRequests.Add(recoveryRequest);
                await  _context.SaveChangesAsync();


            var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    "http://localhost:5005/recover",
                    content
                );

                return response.IsSuccessStatusCode;
            
            //if (string.IsNullOrWhiteSpace(endpoint.RecoveryWebhookUrl))
            //    return false;

            //var url = endpoint.RecoveryWebhookUrl.TrimEnd('/');

            //if (!url.EndsWith("/recover"))
            //    url += "/recover";

            //var response = await _http.PostAsJsonAsync(url, new
            //{
            //    appPoolName = endpoint.AppPoolName
            //});

            //return response.IsSuccessStatusCode;
        }
    }
}
