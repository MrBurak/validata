using data.validata.com.Interfaces.Metadata;
using Moq;


namespace data.validata.test.Metadata
{
    public class MetadataTests
    {
        [Fact]
        public void Constructor_Should_ThrowArgumentNullException_WhenEntitiesIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new com.Metadata.Metadata(null!));
        }

        [Fact]
        public void Constructor_Should_AssignEntitiesPropertyCorrectly()
        {
            var mockEntity = new Mock<IEntity>().Object;
            var dictionary = new Dictionary<Type, IEntity>
        {
            { typeof(string), mockEntity }
        };

            var metadata = new com.Metadata.Metadata(dictionary);

            Assert.Equal(dictionary, metadata.Entities);
            Assert.Single(metadata.Entities);
            Assert.Same(mockEntity, metadata.Entities[typeof(string)]);
        }
    }
}
