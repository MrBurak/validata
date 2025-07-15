using model.validata.com.ValueObjects.OrderItem;

namespace model.validata.test.ValueObjects.OrderItem
{


    public class ItemProductPriceTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var price = new ItemProductPrice(99.99m);
            Assert.Equal(99.99m, price.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10.5)]
        public void Constructor_ShouldThrow_WhenValueIsZeroOrNegative(decimal invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ItemProductPrice(invalidValue));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new ItemProductPrice(49.99m);
            var b = new ItemProductPrice(49.99m);
            var c = new ItemProductPrice(79.99m);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new ItemProductPrice(15.75m);
            var b = new ItemProductPrice(15.75m);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        
    }

}
