using model.validata.com.ValueObjects.Customer;


namespace model.validata.test.ValueObjects.Customer
{


    public class FirstNameTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var firstName = new FirstName("Burak");
            Assert.Equal("Burak", firstName.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new FirstName(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var input = new string('a', 129);
            Assert.Throws<ArgumentException>(() => new FirstName(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var name1 = new FirstName("Burak");
            var name2 = new FirstName("Burak");
            var name3 = new FirstName("Other");

            Assert.Equal(name1, name2);
            Assert.True(name1 == name2);
            Assert.False(name1 != name2);
            Assert.NotEqual(name1, name3);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var name1 = new FirstName("Burak");
            var name2 = new FirstName("Burak");
            Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var name = new FirstName("Burak");
            Assert.Equal("Burak", name.ToString());
        }
    }

}
