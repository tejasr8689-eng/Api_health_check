using Api_health_check.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Api_health_check.Pages.Endpoints
{
    public class DeleteModel : PageModel
    {
        private readonly HealthCheckContext _context;

        public DeleteModel(HealthCheckContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MonitoredEndpoint Endpoint { get; set; } = default!;

        // STEP 1: Load endpoint for confirmation
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Endpoint = await _context.MonitoredEndpoints
                .FirstOrDefaultAsync(e => e.Id == id);

            if (Endpoint == null)
                return NotFound();

            return Page();
        }

        // STEP 2: Delete after confirmation
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var endpoint = await _context.MonitoredEndpoints.FindAsync(id);

            if (endpoint != null)
            {
                _context.MonitoredEndpoints.Remove(endpoint);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }
    }
}
