using model.validata.com.ValueObjects.Product;

namespace model.validata.test.ValueObjects.Product
{


    public class ProductNameTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var name = new ProductName("Test Product");
            Assert.Equal("Test Product", name.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new ProductName(input));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenTooLong()
        {
            var longName = new string('a', 129);
            Assert.Throws<ArgumentException>(() => new ProductName(longName));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new ProductName("Product A");
            var b = new ProductName("Product A");
            var c = new ProductName("Product B");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new ProductName("Match");
            var b = new ProductName("Match");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ImplicitConversionToString_ShouldReturnValue()
        {
            var name = new ProductName("Converted Product");
            string value = name;
            Assert.Equal("Converted Product", value);
        }

        [Fact]
        public void ExplicitConversionFromString_ShouldCreateProductName()
        {
            string input = "Explicit Product";
            var name = (ProductName)input;
            Assert.Equal("Explicit Product", name.Value);
        }

        
    }

}
