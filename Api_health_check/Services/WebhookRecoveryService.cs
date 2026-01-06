using System.Text;

namespace Api_health_check.Services
{
    public class WebhookRecoveryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WebhookRecoveryService> _logger;
        public WebhookRecoveryService(HttpClient httpClient,ILogger<WebhookRecoveryService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> TriggerAsync(string webhookUrl)
        {
            try
            {
                _logger.LogCritical("🔥 WEBHOOK POST STARTED → {Url}", webhookUrl);

                var payload = new StringContent(
                    $"{{ \"source\": \"Api_health_check\", \"time\": \"{DateTime.UtcNow:u}\" }}",
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(webhookUrl, payload);

                _logger.LogInformation(
                    "Webhook response status: {StatusCode}",
                    response.StatusCode);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Exception ex1 = ex;
                _logger.LogError(ex1, "Webhook POST failed");
                return false;
            }
        }
    }

}
