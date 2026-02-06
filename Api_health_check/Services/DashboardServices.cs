using Api_health_check.Data;
using Api_health_check.DTOs.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_health_check.Services
{

    public class DashboardServices
    {
        private readonly Data.HealthCheckContext _context;  

        public DashboardServices(Data.HealthCheckContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            return new DashboardSummaryDto
            {
                TotalEndpoints = await _context.MonitoredEndpoints.CountAsync(),

                ActiveEndpoints = await _context.MonitoredEndpoints
                 .CountAsync(e => e.IsActive),

                InactiveEndpoints = await _context.MonitoredEndpoints
                 .CountAsync(e => !e.IsActive),

                DownEndpoints = await _context.MonitoredEndpoints
                 .CountAsync(e => e.IsDown)
            };
        }
    }
}
