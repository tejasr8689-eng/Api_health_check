using Api_health_check.Data;
using System;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
            
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "X-Recovery-Key",
                    "tejasrapikeyforhealthcheckapi@8689973corm"
                );

                var payload = new
                {
                    action = endpoint.RecoveryAction,          // 👈 user provided
                    target = endpoint.AppPoolName      // 👈 user provided
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    "http://localhost:5005/recover",
                    content
                );

                return response.IsSuccessStatusCode;
            
            //if (string.IsNullOrWhiteSpace(endpoint.RecoveryWebhookUrl))
            //    return false;

            //var url = endpoint.RecoveryWebhookUrl.TrimEnd('/');

            //if (!url.EndsWith("/recover"))
            //    url += "/recover";

            //var response = await _http.PostAsJsonAsync(url, new
            //{
            //    appPoolName = endpoint.AppPoolName
            //});

            //return response.IsSuccessStatusCode;
        }
    }
}
