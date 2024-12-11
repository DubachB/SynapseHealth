using Newtonsoft.Json.Linq;

namespace Synapse.OrdersExample.Services
{
    public interface IOrderService
    {
        Task<JObject[]> FetchMedicalEquipmentOrders();
        JObject ProcessOrder(JObject order);
        bool IsItemDelivered(JToken item);
    }

    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<JObject[]> FetchMedicalEquipmentOrders()
        {
            _logger.LogInformation("Fetching orders from API...");
            string ordersApiUrl = "https://orders-api.com/orders";
            var response = await _httpClient.GetAsync(ordersApiUrl);

            if (response.IsSuccessStatusCode)
            {
                var ordersData = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Fetched {ordersData.Length} orders.");
                return JArray.Parse(ordersData).ToObject<JObject[]>();
            }
            else
            {
                _logger.LogError("Failed to fetch orders from API.");
                return new JObject[0];
            }
        }

        public JObject ProcessOrder(JObject order)
        {
            var items = order["Items"].ToObject<JArray>();
            _logger.LogInformation($"Fetched {items.Count} items.");
            foreach (var item in items)
            {
                if (IsItemDelivered(item))
                {
                    return order;
                }
            }
            return null;
        }

        public bool IsItemDelivered(JToken item)
        {
            return item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase);
        }
    }
}
