namespace Api_health_check.Models
{
    public class RecoveryAck
    {
        public string requestId { get; set; } = "";
        public string target { get; set; } = "";
        public string action { get; set; } = "";
        public string status { get; set; }  = "";
        public string details { get; set; } = "";

        public DateTime completedAt { get; set; }
    }
}
