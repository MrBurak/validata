using data.validata.com.Entities;
using data.validata.com.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace data.validata.test.Entities.Configuration
{
    public class OperationSourceConfigurationTests
    {
        [Fact]
        public void Configure_ShouldSetIdentityColumnAndNameConstraints()
        {
            var modelBuilder = new ModelBuilder();
            var config = new OperationSourceConfiguration();

            modelBuilder.Entity<OperationSource>(config.Configure);
            var entity = modelBuilder.Model.FindEntityType(typeof(OperationSource));

            var idProp = entity!.FindProperty(nameof(OperationSource.OperationSourceId));
            var nameProp = entity.FindProperty(nameof(OperationSource.Name));

            Assert.NotNull(idProp);
            Assert.True(idProp.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn);

            Assert.NotNull(nameProp);
            Assert.Equal(50, nameProp.GetMaxLength());
            Assert.False(nameProp.IsNullable);
        }
    }
}
