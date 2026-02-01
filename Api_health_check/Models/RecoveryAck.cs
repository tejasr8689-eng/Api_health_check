namespace Api_health_check.Models
{
    public class RecoveryAck
    {
        public string RequestId { get; set; } = "";
        public string target { get; set; } = "";
        public string Action { get; set; } = "";
        public string Status { get; set; }  = "";
        public string Details { get; set; } = "";

        public DateTime CompletedAt { get; set; }
    }
}
