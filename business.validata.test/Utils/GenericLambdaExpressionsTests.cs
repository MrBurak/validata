using business.validata.com.Utils;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using Moq;

namespace business.validata.test.Utils
{
    public class GenericLambdaExpressionsTests
    {
        private readonly Mock<IMetadata> _mockMetadata;
        private readonly GenericLambdaExpressions _genericLambdaExpressions;

        public GenericLambdaExpressionsTests()
        {
            _mockMetadata = new Mock<IMetadata>();

            var mockProductPropertyInfo = typeof(Product).GetProperty("ProductId")!;
            var mockProductPkProperty = new Mock<IProperty>();
            mockProductPkProperty.Setup(p => p.PropertyInfo).Returns(mockProductPropertyInfo);

            var mockProductNamePropertyInfo = typeof(Product).GetProperty("Name")!;
            var mockProductNameProperty = new Mock<IProperty>();
            mockProductNameProperty.Setup(p => p.PropertyInfo).Returns(mockProductNamePropertyInfo);

            var mockProductPricePropertyInfo = typeof(Product).GetProperty("Price")!;
            var mockProductPriceProperty = new Mock<IProperty>();
            mockProductPriceProperty.Setup(p => p.PropertyInfo).Returns(mockProductPricePropertyInfo);

            var mockProductEntityType = new Mock<IEntity>();
            mockProductEntityType.Setup(e => e.PrimaryKey).Returns(mockProductPkProperty.Object);
            mockProductEntityType.Setup(e => e.Properties)
                .Returns(new Dictionary<string, IProperty>
                {
                { "ProductId", mockProductPkProperty.Object },
                { "Name", mockProductNameProperty.Object },
                { "Price", mockProductPriceProperty.Object }
                });

            _mockMetadata.Setup(m => m.Entities)
                .Returns(new Dictionary<Type, IEntity>
                {
                { typeof(Product), mockProductEntityType.Object }
                });

            _genericLambdaExpressions = new GenericLambdaExpressions(_mockMetadata.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfMetadataIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GenericLambdaExpressions(null!));
        }

        [Fact]
        public void GetEntityByPrimaryKey_ReturnsCorrectExpression()
        {
            var product = new Product { ProductId = 10, Name = "Laptop", Price = 1200.0f };

            var expression = _genericLambdaExpressions.GetEntityByPrimaryKey(product);

            Assert.NotNull(expression);
            Assert.Equal("e => ((e.ProductId == 10) And (e.DeletedOn == null))", expression.ToString());

            var compiled = expression.Compile();
            Assert.True(compiled(new Product { ProductId = 10, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 11, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 10, DeletedOn = DateTime.UtcNow }));
        }

        [Fact]
        public void GetEntityById_ReturnsCorrectExpression()
        {
            int productId = 5;

            var expression = _genericLambdaExpressions.GetEntityById<Product>(productId);

            Assert.NotNull(expression);
            Assert.Equal("e => ((e.ProductId == 5) And (e.DeletedOn == null))", expression.ToString());

            var compiled = expression.Compile();
            Assert.True(compiled(new Product { ProductId = 5, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 6, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 5, DeletedOn = DateTime.UtcNow }));
        }

        [Fact]
        public void GetEntityByUniqueValue_ReturnsCorrectExpression_ForStringField()
        {
            var product = new Product { ProductId = 1, Name = "UniqueProduct", Price = 50.0f };
            var fieldName = "Name";
            var value = "UniqueProduct";
            var ids = new List<int> { 1 };

            var expression = _genericLambdaExpressions.GetEntityByUniqueValue(product, fieldName, value, ids);

            Assert.NotNull(expression);

            
            Assert.Equal("e => (((e.Name == \"UniqueProduct\") And (e.DeletedOn == null)) AndAlso Invoke(e => Not(value(System.Collections.Generic.List`1[System.Int32]).Contains(e.ProductId)), e))", expression.ToString());

            var compiled = expression.Compile();

            Assert.False(compiled(new Product { ProductId = 1, Name = "UniqueProduct", DeletedOn = null }));
            Assert.True(compiled(new Product { ProductId = 2, Name = "UniqueProduct", DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 1, Name = "Another Name", DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 3, Name = "Another Name", DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 2, Name = "UniqueProduct", DeletedOn = DateTime.UtcNow }));
        }

        [Fact]
        public void GetEntityByUniqueValue_ReturnsCorrectExpression_ForFloatField()
        {
            var product = new Product { ProductId = 1, Name = "Test", Price = 30.5f };
            var fieldName = "Price";
            var value = "30.5";
            var ids = new List<int> { 1 };

            var expression = _genericLambdaExpressions.GetEntityByUniqueValue(product, fieldName, value, ids);

            Assert.NotNull(expression);
            Assert.Contains("e.Price == 30.5", expression.ToString());
            Assert.Contains("Not(value(System.Collections.Generic.List`1[System.Int32]).Contains(e.ProductId))", expression.ToString());

            var compiled = expression.Compile();

            Assert.False(compiled(new Product { ProductId = 1, Price = 30.5f, DeletedOn = null }));
            Assert.True(compiled(new Product { ProductId = 2, Price = 30.5f, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 3, Price = 25.0f, DeletedOn = null }));
            Assert.False(compiled(new Product { ProductId = 2, Price = 30.5f, DeletedOn = DateTime.UtcNow }));
        }

        [Fact]
        public void GetDefaultValue_ReturnsCorrectDefaultForValueTypes()
        {
            Assert.Equal(0, GenericLambdaExpressions.GetDefaultValue(typeof(int)));
            Assert.Equal(0.0f, GenericLambdaExpressions.GetDefaultValue(typeof(float)));
            Assert.Equal(default(DateTime), GenericLambdaExpressions.GetDefaultValue(typeof(DateTime)));
            Assert.Equal(false, GenericLambdaExpressions.GetDefaultValue(typeof(bool)));
        }

        [Fact]
        public void GetDefaultValue_ReturnsNullForReferenceTypes()
        {
            Assert.Null(GenericLambdaExpressions.GetDefaultValue(typeof(string)));
            Assert.Null(GenericLambdaExpressions.GetDefaultValue(typeof(object)));
            Assert.Null(GenericLambdaExpressions.GetDefaultValue(typeof(Product)));
        }
    }
}
