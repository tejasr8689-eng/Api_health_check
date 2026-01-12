using Api_health_check.Clients;
using Api_health_check.Data;

namespace Api_health_check.Services
{
    public class RecoveryOrchestratorService
    {
        private readonly RetryPolicyService _retry;
        private readonly IRecoveryClient _recoveryClient;
        private readonly AuditLogService _audit;

        public RecoveryOrchestratorService(
            RetryPolicyService retry,
            IRecoveryClient recoveryClient,
            AuditLogService audit)
        {
            _retry = retry;
            _recoveryClient = recoveryClient;
            _audit = audit;
        }

        public async Task RecoverAsync(MonitoredEndpoint endpoint)
        {
            bool recoveryResult = await _retry.ExecuteAsync(() =>
                _recoveryClient.TriggerRecoveryAsync(endpoint));

            await _audit.TaskAsync(
                endpoint.Id,
                "RECOVERY_API",
                recoveryResult,
                recoveryResult
                    ? "Recovery API triggered"
                    : "Recovery API failed"
            );
        }
    }
}
