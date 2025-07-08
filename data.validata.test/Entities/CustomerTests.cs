using data.validata.com.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.validata.test.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Properties_Should_BeSettableAndGettable()
        {
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Address = "123 Main St",
                Pobox = "12345"
            };

            Assert.Equal(1, customer.CustomerId);
            Assert.Equal("John", customer.FirstName);
            Assert.Equal("Doe", customer.LastName);
            Assert.Equal("john.doe@example.com", customer.Email);
            Assert.Equal("123 Main St", customer.Address);
            Assert.Equal("12345", customer.Pobox);
        }

        [Fact]
        public void CustomerId_Should_HaveKeyAndDatabaseGeneratedAttributes()
        {
            var prop = typeof(Customer).GetProperty(nameof(Customer.CustomerId));
            var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var dbGenAttr = prop.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false)
                               .Cast<DatabaseGeneratedAttribute>()
                               .FirstOrDefault();

            Assert.NotNull(keyAttr);
            Assert.NotNull(dbGenAttr);
            Assert.Equal(DatabaseGeneratedOption.Identity, dbGenAttr.DatabaseGeneratedOption);
        }

        [Theory]
        [InlineData(nameof(Customer.FirstName), 128)]
        [InlineData(nameof(Customer.LastName), 128)]
        [InlineData(nameof(Customer.Email), 128)]
        [InlineData(nameof(Customer.Address), 512)]
        [InlineData(nameof(Customer.Pobox), 10)]
        public void Properties_Should_HaveRequiredAndMaxLength(string propertyName, int maxLength)
        {
            var prop = typeof(Customer).GetProperty(propertyName);

            var requiredAttr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            var maxLengthAttr = prop.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                                   .Cast<MaxLengthAttribute>()
                                   .FirstOrDefault();

            Assert.NotNull(requiredAttr);
            Assert.NotNull(maxLengthAttr);
            Assert.Equal(maxLength, maxLengthAttr.Length);
        }
    }
}
