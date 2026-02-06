using Api_health_check.Data;
using Api_health_check.DTOs.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Api_health_check.Pages.Endpoints
{
    public class  CreateModel : PageModel
    {
        private readonly HealthCheckContext _context;

        public CreateModel(HealthCheckContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CreateEndPointsDto Endpoint { get; set; } = default!; // do not new() here
        public SelectList RecoveryAction { get; private set; }

        public void OnGet()
        {
            // provide defaults so the Razor form and generated code see non-null values
            Endpoint = new CreateEndPointsDto();
           
            RecoveryAction = new SelectList(new[]
                {
                new { Value = 0, Text = "Restart App Pool" },
                new { Value = 1, Text = "Start Website" },
                new { Value = 2, Text = "Restart Website" },
                new { Value = 3, Text = "Full Recovery" }
                }, "Value", "Text");  // No selectedValue = empty selection
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            var entity = new MonitoredEndpoint
            {
                Name = Endpoint.Name,
                Url = Endpoint.Url,
                CheckIntervalSeconds = Endpoint.CheckIntervalSeconds,
                NotifyEmail = Endpoint.NotifyEmail,
                AppPoolName = Endpoint.AppPoolName,
                RecoveryWebhookUrl = Endpoint.RecoveryWebhookUrl,
                IsActive = Endpoint.IsActive,
                RecoveryAction = Endpoint.RecoveryAction,

                // SYstem managedfields
                 IsDown = false,
                 ConsecutiveFailures = 0,
                 LastCheckUtc = null,
                 LastRecoveryUtc = null
            };

            _context.MonitoredEndpoints.Add(entity);
            await _context.SaveChangesAsync();

            // Later you can redirect to a dashboard; for now go home
            return RedirectToPage("/Index");
        }
    }
}
