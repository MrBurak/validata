using model.validata.com.DTO;


namespace model.validata.test.DTO
{
    public class OrderDtoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var order = new OrderDto
            {
                OrderId = 1,
                OrderDate = new DateTime(2025, 7, 14),
                TotalAmount = 150.75m,
                ProductQuantity = 3,
                CustomerId = 10
            };

            Assert.Equal(1, order.OrderId);
            Assert.Equal(new DateTime(2025, 7, 14), order.OrderDate);
            Assert.Equal(150.75m, order.TotalAmount);
            Assert.Equal(3, order.ProductQuantity);
            Assert.Equal(10, order.CustomerId);
        }

        [Fact]
        public void DefaultConstructor_ShouldNotThrow()
        {
            var order = new OrderDto();

            Assert.Equal(0, order.OrderId);
            Assert.Equal(default, order.OrderDate);
            Assert.Equal(0m, order.TotalAmount);
            Assert.Equal(0, order.ProductQuantity);
            Assert.Equal(0, order.CustomerId);
        }
    }
}
