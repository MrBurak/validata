using business.validata.com.Utils;
using data.validata.com.Interfaces.Metadata;
using model.validata.com.Entities;
using Moq;

namespace business.validata.test.Utils
{
    public class TestEntity : BaseEntity
    {
        public int TestEntityId { get; set; }
        public string? Name { get; set; }
        public float Price { get; set; }
    }
    public class GenericLambdaExpressionsTests
    {
        private readonly Mock<IMetadata> _mockMetadata;
        private readonly GenericLambdaExpressions _genericLambdaExpressions;

        public GenericLambdaExpressionsTests()
        {
            _mockMetadata = new Mock<IMetadata>();

            var mockTestEntityPropertyInfo = typeof(TestEntity).GetProperty("TestEntityId")!;
            var mockTestEntityPkProperty = new Mock<IProperty>();
            mockTestEntityPkProperty.Setup(p => p.PropertyInfo).Returns(mockTestEntityPropertyInfo);

            var mockTestEntityNamePropertyInfo = typeof(TestEntity).GetProperty("Name")!;
            var mockTestEntityNameProperty = new Mock<IProperty>();
            mockTestEntityNameProperty.Setup(p => p.PropertyInfo).Returns(mockTestEntityNamePropertyInfo);

            var mockTestEntityPricePropertyInfo = typeof(TestEntity).GetProperty("Price")!;
            var mockTestEntityPriceProperty = new Mock<IProperty>();
            mockTestEntityPriceProperty.Setup(p => p.PropertyInfo).Returns(mockTestEntityPricePropertyInfo);

            var mockTestEntityEntityType = new Mock<IEntity>();
            mockTestEntityEntityType.Setup(e => e.PrimaryKey).Returns(mockTestEntityPkProperty.Object);
            mockTestEntityEntityType.Setup(e => e.Properties)
                .Returns(new Dictionary<string, IProperty>
                {
                { "TestEntityId", mockTestEntityPkProperty.Object },
                { "Name", mockTestEntityNameProperty.Object },
                { "Price", mockTestEntityPriceProperty.Object }
                });

            _mockMetadata.Setup(m => m.Entities)
                .Returns(new Dictionary<Type, IEntity>
                {
                { typeof(TestEntity), mockTestEntityEntityType.Object }
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
            var TestEntity = new TestEntity { TestEntityId = 10, Name = "Laptop", Price = 1200.0f };

            var expression = _genericLambdaExpressions.GetEntityByPrimaryKey(TestEntity);

            Assert.NotNull(expression);
            Assert.Equal("e => ((e.TestEntityId == 10) And (e.DeletedOn == null))", expression.ToString());

            var compiled = expression.Compile();
            Assert.True(compiled(new TestEntity { TestEntityId = 10, DeletedOn = null }));
            Assert.False(compiled(new TestEntity { TestEntityId = 11, DeletedOn = null }));
            Assert.False(compiled(new TestEntity { TestEntityId = 10, DeletedOn = DateTime.UtcNow }));
        }

        [Fact]
        public void GetEntityById_ReturnsCorrectExpression()
        {
            int TestEntityId = 5;

            var expression = _genericLambdaExpressions.GetEntityById<TestEntity>(TestEntityId);

            Assert.NotNull(expression);
            Assert.Equal("e => ((e.TestEntityId == 5) And (e.DeletedOn == null))", expression.ToString());

            var compiled = expression.Compile();
            Assert.True(compiled(new TestEntity { TestEntityId = 5, DeletedOn = null }));
            Assert.False(compiled(new TestEntity { TestEntityId = 6, DeletedOn = null }));
            Assert.False(compiled(new TestEntity { TestEntityId = 5, DeletedOn = DateTime.UtcNow }));
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
            Assert.Null(GenericLambdaExpressions.GetDefaultValue(typeof(TestEntity)));
        }
    }
}
