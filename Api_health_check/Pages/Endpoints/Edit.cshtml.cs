using Api_health_check.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Api_health_check.Pages.Endpoints
{
    public class EditModel : PageModel
    {
        private readonly HealthCheckContext _context;

        public EditModel(HealthCheckContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MonitoredEndpoint Endpoint { get; set; } = default!;
        public SelectList RecoveryAction { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endpoint = await _context.MonitoredEndpoints.FindAsync(id.Value);
            if (endpoint == null)
            {
                return NotFound();
            }

            Endpoint = endpoint;
            RecoveryAction = new SelectList(new[]
               {
                new { Value = 0, Text = "Restart App Pool" },
                new { Value = 1, Text = "Start Website" },
                new { Value = 2, Text = "Restart Website" },
                new { Value = 3, Text = "Full Recovery" }
                }, "Value", "Text");  // No selectedValue = empty selection
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Endpoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MonitoredEndpoints.AnyAsync(e => e.Id == Endpoint.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
                    