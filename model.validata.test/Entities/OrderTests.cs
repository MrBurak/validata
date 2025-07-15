using model.validata.com.ValueObjects.Order;
using model.validata.com.ValueObjects.OrderItem;


namespace model.validata.test.Entities
{
 

    public class OrderTests
    {
        private TotalAmount ValidTotalAmount() => new TotalAmount(100.50m);
        private ProductQuantity ValidProductQuantity() => new ProductQuantity(5);

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var orderDate = DateTime.UtcNow;
            var totalAmount = ValidTotalAmount();
            var productQuantity = ValidProductQuantity();
            var order = new com.Entities.Order(1, 10, orderDate, totalAmount, productQuantity);

            Assert.Equal(1, order.OrderId);
            Assert.Equal(10, order.CustomerId);
            Assert.Equal(orderDate, order.OrderDate);
            Assert.Equal(orderDate, order.OrderDateValue);
            Assert.Equal(totalAmount.Value, order.TotalAmountValue);
            Assert.Equal(productQuantity.Value, order.ProductQuantityValue);
            Assert.NotNull(order.OrderItems);
            Assert.Empty(order.OrderItems);
        }

        [Fact]
        public void Constructor_ShouldThrowForInvalidCustomerId()
        {
            var orderDate = DateTime.UtcNow;
            var totalAmount = ValidTotalAmount();
            var productQuantity = ValidProductQuantity();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new com.Entities.Order(1, -1, orderDate, totalAmount, productQuantity));
        }

        [Fact]
        public void Constructor_ShouldThrowForDefaultOrderDate()
        {
            var totalAmount = ValidTotalAmount();
            var productQuantity = ValidProductQuantity();

            Assert.Throws<ArgumentException>(() =>
                new com.Entities.Order(1, 10, default, totalAmount, productQuantity));
        }

        [Fact]
        public void UpdateTotalAmount_ShouldUpdateValueAndTimestamp()
        {
            var order = new com.Entities.Order(1, 10, DateTime.UtcNow, ValidTotalAmount(), ValidProductQuantity());
            var newTotal = new TotalAmount(200.00m);
            var oldTimestamp = order.LastModifiedTimeStamp;

            order.UpdateTotalAmount(newTotal);

            Assert.Equal(newTotal.Value, order.TotalAmountValue);
            Assert.Equal(newTotal.Value, order.TotalAmount.Value);
            Assert.True(order.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void UpdateProductCount_ShouldUpdateValueAndTimestamp()
        {
            var order = new com.Entities.Order(1, 10, DateTime.UtcNow, ValidTotalAmount(), ValidProductQuantity());
            var newQuantity = new ProductQuantity(10);
            var oldTimestamp = order.LastModifiedTimeStamp;

            order.UpdateProductCount(newQuantity);

            Assert.Equal(newQuantity.Value, order.ProductQuantityValue);
            Assert.Equal(newQuantity.Value, order.ProductQuantity.Value);
            Assert.True(order.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void AddOrderItem_ShouldAddItemAndUpdateTimestamp()
        {
            var order = new com.Entities.Order(1, 10, DateTime.UtcNow, ValidTotalAmount(), ValidProductQuantity());
            var orderItem = new com.Entities.OrderItem(1, order.OrderId, new ItemProductQuantity(2), new ItemProductPrice(50.00m));
            var oldTimestamp = order.LastModifiedTimeStamp;

            order.AddOrderItem(orderItem);

            Assert.Contains(orderItem, order.OrderItems);
            Assert.True(order.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void AddOrderItem_ShouldThrowForNull()
        {
            var order = new com.Entities.Order(1, 10, DateTime.UtcNow, ValidTotalAmount(), ValidProductQuantity());
            Assert.Throws<ArgumentNullException>(() => order.AddOrderItem(null!));
        }

        [Fact]
        public void LoadValueObjectsFromBackingFields_ShouldInitializeValueObjects()
        {
            var orderDate = DateTime.UtcNow;
            var totalAmount = ValidTotalAmount();
            var productQuantity = ValidProductQuantity();

            var order = new com.Entities.Order(1, 10, orderDate, totalAmount, productQuantity);

            
            order.GetType().GetProperty("OrderDate")!.SetValue(order, default(DateTime));
            order.GetType().GetProperty("TotalAmount")!.SetValue(order, null);
            order.GetType().GetProperty("ProductQuantity")!.SetValue(order, null);

            order.LoadValueObjectsFromBackingFields();

            Assert.Equal(orderDate, order.OrderDate);
            Assert.Equal(totalAmount.Value, order.TotalAmount.Value);
            Assert.Equal(productQuantity.Value, order.ProductQuantity.Value);
        }
    }

}
