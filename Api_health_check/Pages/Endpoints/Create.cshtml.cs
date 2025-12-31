using System.Threading.Tasks;
using Api_health_check.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Api_health_check.Pages.Endpoints
{
    public class CreateModel : PageModel
    {
        private readonly HealthCheckContext _context;

        public CreateModel(HealthCheckContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MonitoredEndpoint Endpoint { get; set; } = new MonitoredEndpoint();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MonitoredEndpoints.Add(Endpoint);
            await _context.SaveChangesAsync();

            // Later you can redirect to a dashboard; for now go home
            return RedirectToPage("/Index");
        }
    }
}
