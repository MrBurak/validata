using model.validata.com.ValueObjects.Customer;

namespace model.validata.test.ValueObjects.Customer
{


    public class PostalCodeTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var code = new PostalCode("12345-AB");
            Assert.Equal("12345-AB", code.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new PostalCode(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var input = new string('1', 11);
            Assert.Throws<ArgumentException>(() => new PostalCode(input));
        }

        [Theory]
        [InlineData("12345@")]
        [InlineData("ABC!")]
        [InlineData("%%##")]
        public void Constructor_ShouldThrow_WhenInvalidCharacters(string input)
        {
            Assert.Throws<ArgumentException>(() => new PostalCode(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new PostalCode("12345");
            var b = new PostalCode("12345");
            var c = new PostalCode("54321");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new PostalCode("A1B2C3");
            var b = new PostalCode("A1B2C3");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var code = new PostalCode("ABC 123");
            Assert.Equal("ABC 123", code.ToString());
        }
    }

}
