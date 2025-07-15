using model.validata.com.ValueObjects.Customer;

namespace model.validata.test.ValueObjects.Customer
{

    public class LastNameTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var lastName = new LastName("Kepti");
            Assert.Equal("Kepti", lastName.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new LastName(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var input = new string('a', 129);
            Assert.Throws<ArgumentException>(() => new LastName(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var name1 = new LastName("Kepti");
            var name2 = new LastName("Kepti");
            var name3 = new LastName("Other");

            Assert.Equal(name1, name2);
            Assert.True(name1 == name2);
            Assert.False(name1 != name2);
            Assert.NotEqual(name1, name3);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var name1 = new LastName("Kepti");
            var name2 = new LastName("Kepti");
            Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var name = new LastName("Kepti");
            Assert.Equal("Kepti", name.ToString());
        }
    }

}
