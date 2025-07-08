using data.validata.com.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.validata.test.Entities
{
    public class OrderItemTests
    {
        [Fact]
        public void Properties_Should_BeSettableAndGettable()
        {
            var product = new Product();
            var order = new Order();

            var orderItem = new OrderItem
            {
                OrderItemId = 1,
                Quantity = 10,
                ProductPrice = 25.5f,
                ProductId = 100,
                Product = product,
                OrderId = 200,
                Order = order
            };

            Assert.Equal(1, orderItem.OrderItemId);
            Assert.Equal(10, orderItem.Quantity);
            Assert.Equal(25.5f, orderItem.ProductPrice);
            Assert.Equal(100, orderItem.ProductId);
            Assert.Equal(product, orderItem.Product);
            Assert.Equal(200, orderItem.OrderId);
            Assert.Equal(order, orderItem.Order);
        }

        [Fact]
        public void OrderItemId_Should_HaveKeyAndDatabaseGeneratedAttributes()
        {
            var prop = typeof(OrderItem).GetProperty(nameof(OrderItem.OrderItemId));
            var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var dbGenAttr = prop.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false)
                               .Cast<DatabaseGeneratedAttribute>()
                               .FirstOrDefault();

            Assert.NotNull(keyAttr);
            Assert.NotNull(dbGenAttr);
            Assert.Equal(DatabaseGeneratedOption.Identity, dbGenAttr.DatabaseGeneratedOption);
        }

        [Theory]
        [InlineData(nameof(OrderItem.Quantity))]
        [InlineData(nameof(OrderItem.ProductPrice))]
        [InlineData(nameof(OrderItem.ProductId))]
        [InlineData(nameof(OrderItem.OrderId))]
        public void Properties_Should_HaveRequiredAttribute(string propertyName)
        {
            var prop = typeof(OrderItem).GetProperty(propertyName);
            var requiredAttr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            Assert.NotNull(requiredAttr);
        }
    }
}
