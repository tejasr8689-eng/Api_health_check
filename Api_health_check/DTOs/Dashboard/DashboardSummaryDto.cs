namespace Api_health_check.DTOs.Dashboard

{
    public class DashboardSummaryDto
    {
        public int TotalEndpoints { get; set; }
        public int ActiveEndpoints { get; set; }
        public int InactiveEndpoints { get; set; }
        public int DownEndpoints { get; set; }
        
    }
}
