using model.validata.com;


namespace model.validata.test
{


    public class PaginationRequestTests
    {
        [Fact]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            var request = new PaginationRequest(2, 10);

            Assert.Equal(2, request.pageNumber);
            Assert.Equal(10, request.pageSize);
            Assert.Equal(10, request.offset);
        }

        [Theory]
        [InlineData(1, 10, 0)]
        [InlineData(3, 20, 40)]
        [InlineData(5, 5, 20)]
        public void Offset_ShouldBeCalculatedCorrectly(int pageNumber, int pageSize, int expectedOffset)
        {
            var request = new PaginationRequest(pageNumber, pageSize);

            Assert.Equal(expectedOffset, request.offset);
        }
    }

}
