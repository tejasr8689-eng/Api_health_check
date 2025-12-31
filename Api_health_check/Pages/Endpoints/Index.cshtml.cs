using System.Collections.Generic;
using System.Threading.Tasks;
using Api_health_check.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Api_health_check.Pages.Endpoints
{
    public class Index : PageModel
    {
        private readonly HealthCheckContext _context;

        public Index (HealthCheckContext context)
        {
            _context = context;
        }

        public IList<MonitoredEndpoint> Endpoints { get; set; } = new List<MonitoredEndpoint>();

        public async Task OnGetAsync()
        {
            Endpoints = await _context.MonitoredEndpoints
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
