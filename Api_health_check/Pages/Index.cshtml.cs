using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Api_health_check.Services;
using Api_health_check.DTOs.Dashboard;

namespace Api_health_check.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DashboardServices _dashboardServices;

        public IndexModel(ILogger<IndexModel> logger, DashboardServices dashboardServices)
        {
            _logger = logger;
            _dashboardServices = dashboardServices;
        }
        public DashboardSummaryDto Summary { get; set; }

        public async Task OnGetAsync()
        {
            Summary = await _dashboardServices.GetSummaryAsync();
        }
    }
}
