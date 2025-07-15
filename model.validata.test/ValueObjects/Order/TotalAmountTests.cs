using model.validata.com.ValueObjects.Order;

namespace model.validata.test.ValueObjects.Order
{


    public class TotalAmountTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var amount = new TotalAmount(100.50m);
            Assert.Equal(100.50m, amount.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99.99)]
        public void Constructor_ShouldThrow_WhenValueIsZeroOrNegative(decimal invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TotalAmount(invalidValue));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new TotalAmount(75.25m);
            var b = new TotalAmount(75.25m);
            var c = new TotalAmount(80.00m);

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatch_ForEqualObjects()
        {
            var a = new TotalAmount(42.42m);
            var b = new TotalAmount(42.42m);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        
    }

}
