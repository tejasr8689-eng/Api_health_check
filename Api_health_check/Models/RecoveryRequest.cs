namespace Api_health_check.Models
{
    public class RecoveryRequest
    {
        public int Id { get; set; }
        public string RequestId { get; set; } = "";
        public int EndpointId { get; set; }
        public string action { get; set; } = "";
        public string target { get; set; } = "";
        public DateTime requestedAt { get; set; }
        public string status { get; set; } = "sent"; // pending, in_progress, completed, failed

    }
}
