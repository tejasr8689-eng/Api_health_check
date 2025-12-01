using System;
using Microsoft.EntityFrameworkCore;

namespace Api_health_check.Data
{
    public class MonitoredEndpoint
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int CheckIntervalSeconds { get; set; } = 60;
        public int? LastStatusCode { get; set; }
        public DateTime? LastCheckUtc { get; set; }
        public bool IsDown { get; set; }
        public int ConsecutiveFailures { get; set; }
        public string NotifyEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
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
