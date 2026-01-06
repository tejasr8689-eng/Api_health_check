using Api_health_check.Data;
namespace Api_health_check.Services
{
    public class AuditLogService
    {
        private readonly Data.HealthCheckContext _context;
        public AuditLogService(Data.HealthCheckContext context)
        {
            _context = context;
        }
        public async Task TaskAsync(int endpointId, string action, bool success, string message)
        {
            var log = new Models.RecoveryAuditLog
            {
                EndpointId = endpointId,
                Action = action,
                Success = success,
                Message = message,
                AttemptedAtUtc = DateTime.UtcNow
            };
            _context.RecoveryAuditLos.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
