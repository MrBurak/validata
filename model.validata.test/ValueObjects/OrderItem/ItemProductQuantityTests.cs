using model.validata.com.ValueObjects.OrderItem;

namespace model.validata.test.ValueObjects.OrderItem
{


    public class ItemProductQuantityTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var quantity = new ItemProductQuantity(5);
            Assert.Equal(5, quantity.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_ShouldThrow_WhenValueIsZeroOrNegative(int invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ItemProductQuantity(invalidValue));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new ItemProductQuantity(10);
            var b = new ItemProductQuantity(10);
            var c = new ItemProductQuantity(20);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new ItemProductQuantity(7);
            var b = new ItemProductQuantity(7);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        
    }

}
