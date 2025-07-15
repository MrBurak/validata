using model.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using data.validata.com.Metadata;
using Moq;

namespace data.validata.test.Metadata
{
    public class EntityTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            var entityType = typeof(Product);
            var mockProperty1 = new Mock<IProperty>();
            mockProperty1.SetupGet(p => p.PropertyInfo.Name).Returns("ProductId");
            var mockProperty2 = new Mock<IProperty>();
            mockProperty2.SetupGet(p => p.PropertyInfo.Name).Returns("Name");

            var properties = new Dictionary<string, IProperty>
            {
                { "ProductId", mockProperty1.Object },
                { "Name", mockProperty2.Object }
            };

            var entity = new Entity(entityType, properties);

            Assert.Equal(entityType, entity.Type);
            Assert.Equal(properties, entity.Properties);
            Assert.Equal(mockProperty1.Object, entity.PrimaryKey);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenTypeIsNull()
        {
            var properties = new Dictionary<string, IProperty>(); 

            Assert.Throws<ArgumentNullException>(() => new Entity(null!, properties));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenPropertiesIsNull()
        {
            var entityType = typeof(Product);

            Assert.Throws<ArgumentNullException>(() => new Entity(entityType, null!));
        }

        [Fact]
        public void Constructor_SetsPrimaryKeyCorrectly_WhenPrimaryKeyExists()
        {
            var entityType = typeof(Product);
            var mockPrimaryKeyProperty = new Mock<IProperty>();
            mockPrimaryKeyProperty.SetupGet(p => p.PropertyInfo.Name).Returns("ProductId");

            var properties = new Dictionary<string, IProperty>
            {
                { "ProductId", mockPrimaryKeyProperty.Object },
                { "SomeOtherProperty", new Mock<IProperty>().Object }
            };

            var entity = new Entity(entityType, properties);

            Assert.Equal(mockPrimaryKeyProperty.Object, entity.PrimaryKey);

        }

       
        [Fact]
        public void Constructor_ThrowsKeyNotFoundException_WhenPropertiesIsEmpty()
        {
            var entityType = typeof(Product);
            var properties = new Dictionary<string, IProperty>(); 
            Assert.Throws<KeyNotFoundException>(() => new Entity(entityType, properties));
        }
    }
}
