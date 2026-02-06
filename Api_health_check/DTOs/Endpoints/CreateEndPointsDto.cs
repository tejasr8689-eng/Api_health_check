using System.ComponentModel.DataAnnotations;
namespace Api_health_check.DTOs.Endpoints
{
    public class CreateEndPointsDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Url { get; set; } = string.Empty;
        [Range(1, 86400, ErrorMessage = "Check Interval should be betweeen 1 to 86400")]
        public int CheckIntervalSeconds { get; set; } = 60;

        [EmailAddress]
        public string NotifyEmail { get; set; } = string.Empty;

        [Required]
        public string AppPoolName { get; set; } = string.Empty;

        [Required]
        [Url]
        public string RecoveryWebhookUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        [Range(0, 4, ErrorMessage = "Invalid Recovery Action")]
        public int RecoveryAction { get; set; } = 0;

    }
}
