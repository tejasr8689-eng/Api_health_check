namespace Api_health_check.Models
{
    public class RecoveryAuditLog
    {
        public int Id { get; set; }
        public int EndpointId { get; set; }
        public string Action { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime AttemptedAtUtc { get; set; }

    }
}
