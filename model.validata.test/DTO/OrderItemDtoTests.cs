using model.validata.com.DTO;


namespace model.validata.test.DTO
{
    public class OrderItemDtoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var dto = new OrderItemDto
            {
                OrderItemId = 1,
                Quantity = 2,
                ProductPrice = 99.99m,
                ProductId = 5,
                OrderId = 10
            };

            Assert.Equal(1, dto.OrderItemId);
            Assert.Equal(2, dto.Quantity);
            Assert.Equal(99.99m, dto.ProductPrice);
            Assert.Equal(5, dto.ProductId);
            Assert.Equal(10, dto.OrderId);
        }

        [Fact]
        public void DefaultConstructor_ShouldSetDefaults()
        {
            var dto = new OrderItemDto();

            Assert.Equal(0, dto.OrderItemId);
            Assert.Equal(0, dto.Quantity);
            Assert.Equal(0m, dto.ProductPrice);
            Assert.Equal(0, dto.ProductId);
            Assert.Equal(0, dto.OrderId);
        }
    }
}
