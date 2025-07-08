using data.validata.com.Entities;
using System.ComponentModel.DataAnnotations;

namespace data.validata.test.Entities
{
    public class OperationSourceTests
    {
        [Fact]
        public void Properties_Should_BeSettableAndGettable()
        {
            var operationSource = new OperationSource
            {
                OperationSourceId = 123,
                Name = "Test Source"
            };

            Assert.Equal(123, operationSource.OperationSourceId);
            Assert.Equal("Test Source", operationSource.Name);
        }

        [Fact]
        public void OperationSourceId_Should_HaveKeyAttribute()
        {
            var prop = typeof(OperationSource).GetProperty(nameof(OperationSource.OperationSourceId));
            var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault();

            Assert.NotNull(keyAttr);
        }
    }
}
