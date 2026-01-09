using Api_health_check.Data;

namespace Api_health_check.Services
{
    public class RecoveryOrchestratorService
    {
        private readonly RetryPolicyService _retry;
        private readonly IisRecoveryService _iis;
        private readonly WebhookRecoveryService _webhook;
        private readonly AuditLogService _audit;

        public RecoveryOrchestratorService(
            RetryPolicyService retry,
            IisRecoveryService iis,
            WebhookRecoveryService webhook,
            AuditLogService audit)
        {
            _retry = retry;
            _iis = iis;
            _webhook = webhook;
            _audit = audit;
        }

        public async Task RecoverAsync(MonitoredEndpoint endpoint)
        {
            // 1️ Try IIS App Pool restart (only if name exists)
            bool iisResult = false;

            if (!string.IsNullOrWhiteSpace(endpoint.AppPoolName))
            {
                iisResult = await _retry.ExecuteAsync(() =>
                    Task.FromResult(_iis.RestartAppPool(endpoint.AppPoolName)));

                var iisMessage = iisResult ? "App pool recycled" : (_iis.LastError ?? "Failed to recycle");
                await _audit.TaskAsync(endpoint.Id, "IIS_RESTART", iisResult, iisMessage);

                if (iisResult) return;
            }

            // 2️ Fallback to webhook (only if URL exists)
            bool webhookResult = false;

            if (!string.IsNullOrWhiteSpace(endpoint.RecoveryWebhookUrl))
            {
                webhookResult = await _retry.ExecuteAsync(() =>
                    _webhook.TriggerAsync(endpoint.RecoveryWebhookUrl));

                await _audit.TaskAsync(
                    endpoint.Id,
                    "WEBHOOK",
                    webhookResult,
                    webhookResult ? "Webhook triggered" : "Webhook failed");
            }

        }
    }

}
