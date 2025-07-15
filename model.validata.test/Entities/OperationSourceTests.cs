using model.validata.com.Entities;
using model.validata.com.ValueObjects.OperationSource;

namespace model.validata.test.Entities
{

    public class OperationSourceTests
    {
        [Fact]
        public void CanCreateOperationSource_WithIdAndName()
        {
            var name = new OperationSourceName("API");
            var operationSource = new OperationSource
            {
                OperationSourceId = 1,
                Name = name
            };

            Assert.Equal(1, operationSource.OperationSourceId);
            Assert.NotNull(operationSource.Name);
            Assert.Equal("API", operationSource.Name!.Value);
        }

        [Fact]
        public void Name_CanBeNull()
        {
            var operationSource = new OperationSource
            {
                OperationSourceId = 2,
                Name = null
            };

            Assert.Null(operationSource.Name);
        }
    }

}
