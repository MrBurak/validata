using data.validata.com.Entities;
using model.validata.com.Entities;

namespace data.validata.test.Metadata
{
    public class PropertyTests
    {
        private class SampleEntity : BaseEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public SampleEntity? RelatedEntity { get; set; }
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenPropertyInfoIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new com.Metadata.Property(null!, true));
        }

        [Fact]
        public void Constructor_ShouldAssignValues_Correctly_ForPrimitiveType()
        {
            var propInfo = typeof(SampleEntity).GetProperty(nameof(SampleEntity.Name))!;
            var property = new com.Metadata.Property(propInfo, isPrimaryKey: false);

            Assert.Equal(propInfo, property.PropertyInfo);
            Assert.False(property.IsPrimaryKey);
            Assert.False(property.IsEntityType);
        }

        [Fact]
        public void Constructor_ShouldAssignValues_Correctly_ForEntityType()
        {
            var propInfo = typeof(SampleEntity).GetProperty(nameof(SampleEntity.RelatedEntity))!;
            var property = new com.Metadata.Property(propInfo, isPrimaryKey: true);

            Assert.Equal(propInfo, property.PropertyInfo);
            Assert.True(property.IsPrimaryKey);
            Assert.True(property.IsEntityType);
        }
    }
}
