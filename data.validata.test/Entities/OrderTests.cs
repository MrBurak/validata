using data.validata.com.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.validata.test.Entities
{
    public class OrderTests
    {
        [Fact]
        public void Properties_Should_BeSettableAndGettable()
        {
            var customer = new Customer();
            var orderItems = new List<OrderItem>();

            var order = new Order
            {
                OrderId = 1,
                OrderDate = new DateTime(2023, 1, 1),
                TotalAmount = 100.5f,
                ProductCount = 3,
                CustomerId = 10,
                Customer = customer,
                OrderItems = orderItems
            };

            Assert.Equal(1, order.OrderId);
            Assert.Equal(new DateTime(2023, 1, 1), order.OrderDate);
            Assert.Equal(100.5f, order.TotalAmount);
            Assert.Equal(3, order.ProductCount);
            Assert.Equal(10, order.CustomerId);
            Assert.Equal(customer, order.Customer);
            Assert.Equal(orderItems, order.OrderItems);
        }

        

        [Fact]
        public void OrderId_Should_HaveKeyAndDatabaseGeneratedAttributes()
        {
            var prop = typeof(Order).GetProperty(nameof(Order.OrderId));
            var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var dbGenAttr = prop.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false)
                               .Cast<DatabaseGeneratedAttribute>()
                               .FirstOrDefault();

            Assert.NotNull(keyAttr);
            Assert.NotNull(dbGenAttr);
            Assert.Equal(DatabaseGeneratedOption.Identity, dbGenAttr.DatabaseGeneratedOption);
        }

        [Theory]
        [InlineData(nameof(Order.OrderDate))]
        [InlineData(nameof(Order.TotalAmount))]
        [InlineData(nameof(Order.ProductCount))]
        [InlineData(nameof(Order.CustomerId))]
        public void Properties_Should_HaveRequiredAttribute(string propertyName)
        {
            var prop = typeof(Order).GetProperty(propertyName);
            var requiredAttr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            Assert.NotNull(requiredAttr);
        }
    }
}
