using model.validata.com.ValueObjects.Product;

namespace model.validata.test.ValueObjects.Product
{


    public class ProductPriceTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var price = new ProductPrice(10.99m);
            Assert.Equal(10.99m, price.Value);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        public void Constructor_ShouldThrow_WhenNegative(decimal invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductPrice(invalidValue));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new ProductPrice(49.99m);
            var b = new ProductPrice(49.99m);
            var c = new ProductPrice(79.99m);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldBeEqual_ForEqualObjects()
        {
            var a = new ProductPrice(5m);
            var b = new ProductPrice(5m);

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ImplicitConversionToDecimal_ShouldReturnValue()
        {
            var price = new ProductPrice(19.95m);
            decimal value = price;
            Assert.Equal(19.95m, value);
        }

        [Fact]
        public void ExplicitConversionFromDecimal_ShouldCreateValueObject()
        {
            decimal raw = 23.50m;
            var price = (ProductPrice)raw;
            Assert.Equal(23.50m, price.Value);
        }

        
    }

}
