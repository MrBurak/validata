using model.validata.com.Entities;
using data.validata.com.Metadata;


namespace data.validata.test.Metadata
{
    public class MetadataFactoryTests
    {
        [Fact]
        public void Create_ShouldReturnMetadata_WithExpectedEntities()
        {
            var metadata = MetadataFactory.Create();

            Assert.NotNull(metadata);
            Assert.NotNull(metadata.Entities);
            Assert.NotEmpty(metadata.Entities);

            foreach (var kvp in metadata.Entities)
            {
                var type = kvp.Key;
                var entity = kvp.Value;

                Assert.True(type.IsSubclassOf(typeof(BaseEntity)));

                Assert.Equal(type, entity.Type);

                var expectedKey = $"{type.Name}Id";
                Assert.Contains(expectedKey, entity.Properties.Keys);

                Assert.Equal(expectedKey, entity.PrimaryKey.PropertyInfo.Name);
            }
        }
    }
}
