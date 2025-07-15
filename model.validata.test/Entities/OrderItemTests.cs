using model.validata.com.Entities;
using model.validata.com.ValueObjects.OrderItem;

namespace model.validata.test.Entities
{


    public class OrderItemTests
    {
        private ItemProductQuantity ValidQuantity() => new ItemProductQuantity(3);
        private ItemProductPrice ValidPrice() => new ItemProductPrice(99.99m);

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var quantity = ValidQuantity();
            var price = ValidPrice();
            var orderItem = new OrderItem(1, 2, quantity, price);

            Assert.Equal(1, orderItem.ProductId);
            Assert.Equal(2, orderItem.OrderId);
            Assert.Equal(quantity.Value, orderItem.QuantityValue);
            Assert.Equal(price.Value, orderItem.ProductPriceValue);
            Assert.Equal(quantity.Value, orderItem.Quantity.Value);
            Assert.Equal(price.Value, orderItem.ProductPrice.Value);
            Assert.NotEqual(default, orderItem.CreatedOnTimeStamp);
            Assert.NotEqual(default, orderItem.LastModifiedTimeStamp);
        }

        [Theory]
        [InlineData(-1, 2)]
        [InlineData(1, -2)]
        public void Constructor_ShouldThrowForNegativeIds(int productId, int orderId)
        {
            var quantity = ValidQuantity();
            var price = ValidPrice();

            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(productId, orderId, quantity, price));
        }

        [Fact]
        public void Constructor_ShouldThrowForNullQuantity()
        {
            var price = ValidPrice();
            Assert.Throws<ArgumentNullException>(() => new OrderItem(1, 2, null!, price));
        }

        [Fact]
        public void Constructor_ShouldThrowForNullPrice()
        {
            var quantity = ValidQuantity();
            Assert.Throws<ArgumentNullException>(() => new OrderItem(1, 2, quantity, null!));
        }

        [Fact]
        public void UpdateQuantity_ShouldChangeQuantityAndTimestamp()
        {
            var orderItem = new OrderItem(1, 2, ValidQuantity(), ValidPrice());
            var newQuantity = new ItemProductQuantity(10);
            var oldTimestamp = orderItem.LastModifiedTimeStamp;

            orderItem.UpdateQuantity(newQuantity);

            Assert.Equal(newQuantity.Value, orderItem.QuantityValue);
            Assert.Equal(newQuantity.Value, orderItem.Quantity.Value);
            Assert.True(orderItem.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void UpdateQuantity_ShouldThrowOnNull()
        {
            var orderItem = new OrderItem(1, 2, ValidQuantity(), ValidPrice());
            Assert.Throws<ArgumentNullException>(() => orderItem.UpdateQuantity(null!));
        }

        [Fact]
        public void UpdateProductPrice_ShouldChangePriceAndTimestamp()
        {
            var orderItem = new OrderItem(1, 2, ValidQuantity(), ValidPrice());
            var newPrice = new ItemProductPrice(150.00m);
            var oldTimestamp = orderItem.LastModifiedTimeStamp;

            orderItem.UpdateProductPrice(newPrice);

            Assert.Equal(newPrice.Value, orderItem.ProductPriceValue);
            Assert.Equal(newPrice.Value, orderItem.ProductPrice.Value);
            Assert.True(orderItem.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void UpdateProductPrice_ShouldThrowOnNull()
        {
            var orderItem = new OrderItem(1, 2, ValidQuantity(), ValidPrice());
            Assert.Throws<ArgumentNullException>(() => orderItem.UpdateProductPrice(null!));
        }

        [Fact]
        public void LoadValueObjectsFromBackingFields_ShouldInitializeValueObjects()
        {
            var orderItem = new OrderItem(1, 2, ValidQuantity(), ValidPrice());

            
            orderItem.GetType().GetProperty("Quantity")!.SetValue(orderItem, null);
            orderItem.GetType().GetProperty("ProductPrice")!.SetValue(orderItem, null);

            orderItem.LoadValueObjectsFromBackingFields();

            Assert.NotNull(orderItem.Quantity);
            Assert.Equal(orderItem.QuantityValue, orderItem.Quantity.Value);

            Assert.NotNull(orderItem.ProductPrice);
            Assert.Equal(orderItem.ProductPriceValue, orderItem.ProductPrice.Value);
        }
    }

}
