using data.validata.com.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace data.validata.test.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Properties_Should_BeSettableAndGettable()
        {
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 99.99f
            };

            Assert.Equal(1, product.ProductId);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal(99.99f, product.Price);
        }

        [Fact]
        public void ProductId_Should_HaveKeyAndDatabaseGeneratedAttributes()
        {
            var prop = typeof(Product).GetProperty(nameof(Product.ProductId));
            var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();
            var dbGenAttr = prop.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false)
                               .Cast<DatabaseGeneratedAttribute>()
                               .FirstOrDefault();

            Assert.NotNull(keyAttr);
            Assert.NotNull(dbGenAttr);
            Assert.Equal(DatabaseGeneratedOption.Identity, dbGenAttr.DatabaseGeneratedOption);
        }

        [Fact]
        public void Name_Should_HaveRequiredAndMaxLengthAttributes()
        {
            var prop = typeof(Product).GetProperty(nameof(Product.Name));
            var requiredAttr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            var maxLengthAttr = prop.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                                   .Cast<MaxLengthAttribute>()
                                   .FirstOrDefault();

            Assert.NotNull(requiredAttr);
            Assert.NotNull(maxLengthAttr);
            Assert.Equal(128, maxLengthAttr.Length);
        }

        [Fact]
        public void Price_Should_HaveRequiredAttribute()
        {
            var prop = typeof(Product).GetProperty(nameof(Product.Price));
            var requiredAttr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

            Assert.NotNull(requiredAttr);
        }
    }
}
