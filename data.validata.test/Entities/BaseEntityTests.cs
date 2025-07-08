using data.validata.com.Entities;
using util.validata.com;

namespace data.validata.test.Entities
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity { }

        [Fact]
        public void Properties_Should_HaveExpectedDefaults_AndBeSettable()
        {
            var entity = new TestEntity();

            var now = DateTime.UtcNow;

            Assert.Null(entity.DeletedOn);
            Assert.Null(entity.OperationSourceId);
            Assert.Null(entity.OperationSource);

            
            entity.CreatedOnTimeStamp = now;
            entity.LastModifiedTimeStamp = now;
            entity.DeletedOn = now;
            entity.OperationSourceId = 5;
            var opSource = new OperationSource();
            entity.OperationSource = opSource;

            Assert.Equal(now, entity.CreatedOnTimeStamp);
            Assert.Equal(now, entity.LastModifiedTimeStamp);
            Assert.Equal(now, entity.DeletedOn);
            Assert.Equal(5, entity.OperationSourceId);
            Assert.Equal(opSource, entity.OperationSource);
        }

        [Fact]
        public void Constants_Should_HaveExpectedValues()
        {
            Assert.Equal("PeriodStart", BaseEntity.PeriodStartShadowProperty);
            Assert.Equal("PeriodEnd", BaseEntity.PeriodEndShadowProperty);
        }
    }
}
