using model.validata.com.ValueObjects.OperationSource;

namespace model.validata.test.ValueObjects.OperationSource
{


    public class OperationSourceNameTests
    {
        [Fact]
        public void Constructor_ShouldSetValue_WhenValid()
        {
            var name = new OperationSourceName("System");
            Assert.Equal("System", name.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_WhenEmpty(string input)
        {
            Assert.Throws<ArgumentException>(() => new OperationSourceName(input));
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            var a = new OperationSourceName("Import");
            var b = new OperationSourceName("Import");
            var c = new OperationSourceName("Manual");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var a = new OperationSourceName("SourceA");
            var b = new OperationSourceName("SourceA");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

       
    }

}
