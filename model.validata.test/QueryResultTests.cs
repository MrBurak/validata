using model.validata.com;
namespace model.validata.test
{
    public class QueryResultTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesToDefaultValues()
        {
            var queryResult = new QueryResult<string>();

            Assert.Null(queryResult.Result);
            Assert.False(queryResult.Success);
            Assert.Null(queryResult.Exception);
        }

        [Fact]
        public void CanSetAndGetResult_ReferenceType()
        {
            var expectedResult = "Some data";
            var queryResult = new QueryResult<string>
            {
                Result = expectedResult
            };

            Assert.Equal(expectedResult, queryResult.Result);
        }

        [Fact]
        public void CanSetAndGetResult_ValueType()
        {
            var expectedResult = 123;
            var queryResult = new QueryResult<int>
            {
                Result = expectedResult
            };

            Assert.Equal(expectedResult, queryResult.Result);
        }

        [Fact]
        public void CanSetAndGetSuccess()
        {
            var queryResult = new QueryResult<bool>
            {
                Success = true
            };

            Assert.True(queryResult.Success);

            queryResult.Success = false;
            Assert.False(queryResult.Success);
        }

        [Fact]
        public void CanSetAndGetException()
        {
            var expectedExceptionMessage = "An error occurred.";
            var queryResult = new QueryResult<object>
            {
                Exception = expectedExceptionMessage
            };

            Assert.Equal(expectedExceptionMessage, queryResult.Exception);

            queryResult.Exception = null;
            Assert.Null(queryResult.Exception);
        }

        [Fact]
        public void QueryResult_WithComplexTypeResult_BehavesCorrectly()
        {
            var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.50m };
            var queryResult = new QueryResult<Product>
            {
                Result = product,
                Success = true,
                Exception = null
            };

            Assert.NotNull(queryResult.Result);
            Assert.Equal(1, queryResult.Result!.ProductId);
            Assert.Equal("Test Product", queryResult.Result.Name);
            Assert.True(queryResult.Success);
            Assert.Null(queryResult.Exception);
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }
    }
}