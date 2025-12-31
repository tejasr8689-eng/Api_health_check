using Api_health_check.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Api_health_check.Pages.Endpoints
{
    public class DeleteModel : PageModel
    {
        private readonly HealthCheckContext _context;

        public DeleteModel (HealthCheckContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MonitoredEndpoint Endpoint { get; set; } = new MonitoredEndpoint();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.MonitoredEndpoints == null)
            {
                return NotFound();
            }
            var endpoint = await _context.MonitoredEndpoints.FindAsync(id);
            if (endpoint != null)
            {
                Endpoint = endpoint;
                _context.MonitoredEndpoints.Remove(Endpoint);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/Index");
        }
    }
}
