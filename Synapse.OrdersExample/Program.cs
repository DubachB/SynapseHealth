using Synapse.OrdersExample.Services;

namespace Synapse.OrdersExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start of App...");

            // Create Dependencies
            var serviceProvider = BuildServiceProvider();
            var orderService = serviceProvider.GetService<OrderService>();
            var alertService = serviceProvider.GetService<AlertService>();
            var updateService = serviceProvider.GetService<UpdateService>();

            var medicalEquipmentOrders = await orderService.FetchMedicalEquipmentOrders();

            foreach (var order in medicalEquipmentOrders)
            {
                var updatedOrder = orderService.ProcessOrder(order);
                if (updatedOrder != null)
                {
                    alertService.SendAlertMessage(updatedOrder["Items"][0], updatedOrder["OrderId"].ToString());
                    updateService.SendAlertAndUpdateOrder(updatedOrder);
                }
            }

            Console.WriteLine("End of App...");
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<HttpClient>()
                .AddSingleton<OrderService>()
                .AddSingleton<AlertService>()
                .AddSingleton<UpdateService>()
                .BuildServiceProvider();
        }
    }
}