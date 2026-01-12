using System.Net.Http.Json;
using Api_health_check.Data;

namespace Api_health_check.Clients
{
    public class RecoveryClient : IRecoveryClient
    {
        private readonly HttpClient _http;

        public RecoveryClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> TriggerRecoveryAsync(MonitoredEndpoint endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint.RecoveryWebhookUrl))
                return false;

            var url = endpoint.RecoveryWebhookUrl.TrimEnd('/');

            if (!url.EndsWith("/recover"))
                url += "/recover";

            var response = await _http.PostAsJsonAsync(url, new
            {
                appPoolName = endpoint.AppPoolName
            });

            return response.IsSuccessStatusCode;
        }
    }
}
