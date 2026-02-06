namespace Api_health_check.DTOs.Endpoints
{
    public class EndpointsDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int CheckIntervalSeconds { get; set; }
        public string NotifyEmail { get; set; } = string.Empty;
        public string AppPoolName { get; set; } = string.Empty;
        public string RecoveryWebhookUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RecoveryAction { get; set; }
        public bool IsDown { get; set; }
        public int consecutiveFailures { get; set; }
        public DateTime? LastCheckUtc { get; set; }
        public DateTime? LastRecoveryUtc { get; set; }
    }
}
