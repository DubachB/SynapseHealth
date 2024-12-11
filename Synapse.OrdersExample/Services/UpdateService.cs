using Newtonsoft.Json.Linq;
using System.Text;

namespace Synapse.OrdersExample.Services
{
    internal class UpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(HttpClient httpClient, ILogger<UpdateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendAlertAndUpdateOrder(JObject order)
        {
            string updateApiUrl = "https://update-api.com/update";
            var content = new StringContent(order.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(updateApiUrl, content);
            _logger.LogInformation($"SendAlertAndUpdateOrder() response: {response}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Updated order sent for processing: OrderId {order["OrderId"]}");
                _logger.LogInformation($"Updated order sent for processing: OrderId {order["OrderId"]}");
            }
            else
            {
                Console.WriteLine($"Failed to send updated order for processing: OrderId {order["OrderId"]}");
                _logger.LogInformation($"Failed to send updated order for processing: OrderId {order["OrderId"]}");
            }
        }
    }
}
