using model.validata.com.ValueObjects.Customer;


namespace model.validata.test.ValueObjects.Customer
{

    public class EmailAddressTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var email = new EmailAddress("test@example.com");
            Assert.Equal("test@example.com", email.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new EmailAddress(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var input = new string('a', 129) + "@example.com";
            Assert.Throws<ArgumentException>(() => new EmailAddress(input));
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missing@domain")]
        [InlineData("@nodomain.com")]
        [InlineData("noatsymbol.com")]
        public void Constructor_ShouldThrow_WhenInvalidFormat(string input)
        {
            Assert.Throws<ArgumentException>(() => new EmailAddress(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var email1 = new EmailAddress("test@example.com");
            var email2 = new EmailAddress("test@example.com");
            var email3 = new EmailAddress("other@example.com");

            Assert.Equal(email1, email2);
            Assert.True(email1 == email2);
            Assert.False(email1 != email2);
            Assert.NotEqual(email1, email3);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var email1 = new EmailAddress("test@example.com");
            var email2 = new EmailAddress("test@example.com");
            Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            var email = new EmailAddress("test@example.com");
            Assert.Equal("test@example.com", email.ToString());
        }
    }

}
