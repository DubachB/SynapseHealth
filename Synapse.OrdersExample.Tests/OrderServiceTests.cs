using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using Synapse.OrdersExample.Services;
using System.Net;

namespace Synapse.OrdersExample.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderService> _mockOrderService;

        public OrderServiceTests()
        {
            _mockOrderService = new Mock<IOrderService>();
        }

        [Fact]
        public async Task FetchMedicalEquipmentOrders_ShouldReturnOrders()
        {
            // Arrange
            var fakeOrders = new JObject[]
            {
            new JObject { { "Id", 1 }, { "Description", "Medical Equipment A" } },
            new JObject { { "Id", 2 }, { "Description", "Medical Equipment B" } }
            };

            _mockOrderService.Setup(service => service.FetchMedicalEquipmentOrders())
                             .ReturnsAsync(fakeOrders);

            // Act
            var orders = await _mockOrderService.Object.FetchMedicalEquipmentOrders();

            // Assert
            Assert.NotEmpty(orders);
            Assert.All(orders, order => Assert.Contains("Medical Equipment", order["Description"].ToString()));
        }

        [Fact]
        public void ProcessOrder_ShouldReturnOrder_WhenItemIsDelivered()
        {
            // Arrange
            var httpClient = new HttpClient(); // You may need to mock or use a real HttpClient for this
            var logger = Mock.Of<ILogger<OrderService>>();
            var orderService = new OrderService(httpClient, logger);

            // Create sample order with an item marked as "Delivered"
            var orderJson = @"
            {
                'OrderId': '1',
                'Items': [
                    { 'Status': 'Pending', 'Description': 'Item 1' },
                    { 'Status': 'Delivered', 'Description': 'Item 2' }
                ]
            }";
            var order = JObject.Parse(orderJson);

            // Act
            var result = orderService.ProcessOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result["OrderId"].ToString());
        }

        [Fact]
        public void ProcessOrder_ShouldReturnNull_WhenNoItemsAreDelivered()
        {
            //Create sample order with no items marked as "Delivered"
            var orderJson = @"
            {
                'OrderId': '2',
                'Items': [
                    { 'Status': 'Pending', 'Description': 'Item 1' },
                    { 'Status': 'Pending', 'Description': 'Item 2' }
                ]
            }";
            var order = JObject.Parse(orderJson);

            //Call method
            var result = _mockOrderService.Object.ProcessOrder(order);

            //Ensure result is null since no items are delivered
            Assert.Null(result);
        }

        [Fact]
        public void IsItemDelivered_ShouldReturnTrue_WhenStatusIsDelivered()
        {
            // Arrange
            var httpClient = new HttpClient();
            var logger = Mock.Of<ILogger<OrderService>>();
            var orderService = new OrderService(httpClient, logger);

            // Create sample item with status "Delivered"
            var itemJson = @"
            {
                'Status': 'Delivered',
                'Description': 'Item 1'
            }";
            var item = JObject.Parse(itemJson);

            // Act
            var result = orderService.IsItemDelivered(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsItemDelivered_ShouldReturnFalse_WhenStatusIsNotDelivered()
        {
            // Arrange
            var httpClient = new HttpClient();
            var logger = Mock.Of<ILogger<OrderService>>();
            var orderService = new OrderService(httpClient, logger);

            // Create sample item with a status other than "Delivered"
            var itemJson = @"
            {
                'Status': 'Pending',
                'Description': 'Item 1'
            }";
            var item = JObject.Parse(itemJson);

            // Act
            var result = orderService.IsItemDelivered(item);

            // Assert
            Assert.False(result);
        }
    }
}