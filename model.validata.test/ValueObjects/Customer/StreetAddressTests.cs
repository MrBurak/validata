using model.validata.com.ValueObjects.Customer;


namespace model.validata.test.ValueObjects.Customer
{


    public class StreetAddressTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var address = new StreetAddress("123 Main Street");
            Assert.Equal("123 Main Street", address.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new StreetAddress(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var input = new string('a', 513);
            Assert.Throws<ArgumentException>(() => new StreetAddress(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new StreetAddress("456 Elm St");
            var b = new StreetAddress("456 Elm St");
            var c = new StreetAddress("789 Oak Ave");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new StreetAddress("10 Downing Street");
            var b = new StreetAddress("10 Downing Street");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var address = new StreetAddress("1600 Pennsylvania Ave");
            Assert.Equal("1600 Pennsylvania Ave", address.ToString());
        }
    }

}
