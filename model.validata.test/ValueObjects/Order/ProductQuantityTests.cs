using model.validata.com.ValueObjects.Order;

namespace model.validata.test.ValueObjects.Order
{


    public class ProductQuantityTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var quantity = new ProductQuantity(10);
            Assert.Equal(10, quantity.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_ShouldThrow_WhenValueIsZeroOrNegative(int invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProductQuantity(invalidValue));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new ProductQuantity(5);
            var b = new ProductQuantity(5);
            var c = new ProductQuantity(6);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldBeEqual_ForEqualObjects()
        {
            var a = new ProductQuantity(12);
            var b = new ProductQuantity(12);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        
    }

}
