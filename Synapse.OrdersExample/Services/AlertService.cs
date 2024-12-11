using Newtonsoft.Json.Linq;
using System.Text;

namespace Synapse.OrdersExample.Services
{
    internal class AlertService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlertService> _logger;

        public AlertService(HttpClient httpClient, ILogger<AlertService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void SendAlertMessage(JToken item, string orderId)
        {
            string alertApiUrl = "https://alert-api.com/alerts";
            var alertData = new
            {
                Message = $"Alert for delivered item: Order {orderId}, Item: {item["Description"]}, " +
                          $"Delivery Notifications: {item["deliveryNotification"]}"
            };

            var content = new StringContent(JObject.FromObject(alertData).ToString(), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(alertApiUrl, content).Result;
            _logger.LogInformation($"SendAlertMessage() response: {response}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Alert sent for delivered item: {item["Description"]}");
                _logger.LogInformation($"Alert sent for delivered item: {item["Description"]}");
            }
            else
            {
                Console.WriteLine($"Failed to send alert for delivered item: {item["Description"]}");
                _logger.LogInformation($"Failed to send alert for delivered item: {item["Description"]}");
            }
        }
    }
}
