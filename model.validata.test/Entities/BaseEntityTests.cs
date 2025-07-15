using model.validata.com.Entities;
using model.validata.com.Enumeration;

namespace model.validata.test.Entities
{


    public class BaseEntityTests
    {
        [Fact]
        public void CreatedOnTimeStamp_ShouldBeSet_ByDefault()
        {
            var entity = new TestEntity();
            Assert.True(entity.CreatedOnTimeStamp != default);
        }

        [Fact]
        public void LastModifiedTimeStamp_ShouldBeSet_ByDefault()
        {
            var entity = new TestEntity();
            Assert.True(entity.LastModifiedTimeStamp != default);
        }

        [Fact]
        public void MarkAsDeleted_ShouldSetDeletedOn_IfNotAlreadySet()
        {
            var entity = new TestEntity();

            entity.MarkAsDeleted();

            Assert.NotNull(entity.DeletedOn);
            Assert.True(entity.LastModifiedTimeStamp != default);
            Assert.Equal((int)BusinessOperationSource.Api, entity.OperationSourceId);
        }

        [Fact]
        public void MarkAsDeleted_ShouldNotOverride_IfAlreadyDeleted()
        {
            var entity = new TestEntity();
            var initialDeletedOn = DateTime.UtcNow.AddHours(-1);
            entity.DeletedOn = initialDeletedOn;

            entity.MarkAsDeleted();

            Assert.Equal(initialDeletedOn, entity.DeletedOn);
        }

        private class TestEntity : BaseEntity { }
    }

}
