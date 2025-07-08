using data.validata.com.Context;
using data.validata.com.Entities;
using System.Linq.Expressions;

namespace data.validata.test.Context
{
    public class TEntityIndexTests
    {
       

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            Expression<Func<Product, object?>> expression = e => e.ProductId;
            var index = new TEntityIndex<Product>
            {
                Expression = expression,
                IsUnique = true,
                Filter = "IsDeleted = 0"
            };

            Assert.Equal(expression, index.Expression);
            Assert.True(index.IsUnique);
            Assert.Equal("IsDeleted = 0", index.Filter);
        }

        [Fact]
        public void DefaultValues_ShouldBeNullOrFalse()
        {
            var index = new TEntityIndex<Product>();

            Assert.Null(index.Expression);
            Assert.False(index.IsUnique);
            Assert.Null(index.Filter);
        }
    }
}
