using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Api_health_check.Data
{
    public class MonitoredEndpoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        [Required]
        [Range(1, 86400, ErrorMessage = "Check interval must be between 1 and 86400 seconds.")]
        public int CheckIntervalSeconds { get; set; } = 60;
        public int? LastStatusCode { get; set; }
        public DateTime? LastCheckUtc { get; set; }
        public bool IsDown { get; set; }
        public int ConsecutiveFailures { get; set; }
        public string NotifyEmail { get; set; } = string.Empty;
        public bool AlertSentForCurrentDown { get; set; }
        public bool IsActive { get; set; } = true;
        public bool NotifyOnRecovery { get; set; } = true; // optional toggle

    }

    public class HealthCheckContext : DbContext
    {
        public HealthCheckContext(DbContextOptions<HealthCheckContext> options)
            : base(options)
        {
        }

        public DbSet<MonitoredEndpoint> MonitoredEndpoints => Set<MonitoredEndpoint>();
    }
}
