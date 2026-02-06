using Api_health_check.Data;
using Api_health_check.DTOs.Endpoints;
using Api_health_check.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Api_health_check.Pages.Endpoints
{
    public class DetailsModel : PageModel
    {
        private readonly HealthCheckContext _context;
        public DetailsModel(HealthCheckContext context)
        {
            _context = context;
        }
        public EndpointsDetailsDto Endpoint { get; set; } = new EndpointsDetailsDto();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MonitoredEndpoint? endpoint = await _context.MonitoredEndpoints.FindAsync(id);
            if (endpoint == null)
            {
                return NotFound();
            }
            Endpoint = new EndpointsDetailsDto
            {
                Name = endpoint.Name,
                Url = endpoint.Url,
                CheckIntervalSeconds = endpoint.CheckIntervalSeconds,
                NotifyEmail = endpoint.NotifyEmail,
                AppPoolName = endpoint.AppPoolName,
                RecoveryWebhookUrl = endpoint.RecoveryWebhookUrl,
                IsActive = endpoint.IsActive,
                RecoveryAction = endpoint.RecoveryAction,
                IsDown = endpoint.IsDown,
                consecutiveFailures = endpoint.ConsecutiveFailures,
                LastCheckUtc = endpoint.LastCheckUtc,
                LastRecoveryUtc = endpoint.LastRecoveryUtc
            };
            return Page();
        }
        
    }
}
