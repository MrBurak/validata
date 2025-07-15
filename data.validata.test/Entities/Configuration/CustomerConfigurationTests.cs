using data.validata.com.Configuration;
using model.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using util.validata.com;

namespace data.validata.test.Entities.Configuration
{
    public class CustomerConfigurationTests
    {
        [Fact]
        public void Configure_ShouldApplyPropertyAndRelationshipConfigurations()
        {
            var builder = new ModelBuilder();
            var config = new CustomerConfiguration();

            builder.Entity<Customer>(config.Configure);
            var model = builder.Model;
            var entity = model.FindEntityType(typeof(Customer));

            var createdOnProp = entity!.FindProperty(nameof(Customer.CreatedOnTimeStamp));
            var lastModifiedProp = entity.FindProperty(nameof(Customer.LastModifiedTimeStamp));
            var foreignKey = entity.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(OperationSource));

            Assert.NotNull(createdOnProp);
            Assert.Equal(DateTimeUtil.DbDate_DataType, createdOnProp.GetColumnType());
            Assert.True(createdOnProp.IsNullable == false);

            Assert.NotNull(lastModifiedProp);
            Assert.Equal(DateTimeUtil.DbDate_DataType, lastModifiedProp.GetColumnType());
            Assert.True(lastModifiedProp.IsNullable == false);

            Assert.NotNull(foreignKey);
            Assert.Equal(nameof(Customer.OperationSourceId), foreignKey.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
            Assert.False(foreignKey.IsRequired);
        }
    }
}
