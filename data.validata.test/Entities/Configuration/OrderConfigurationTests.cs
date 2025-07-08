using data.validata.com.Configuration;
using data.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using util.validata.com;

namespace data.validata.test.Entities.Configuration
{
    public class OrderConfigurationTests
    {
        [Fact]
        public void Configure_ShouldSetPropertiesAndRelationshipsCorrectly()
        {
            var modelBuilder = new ModelBuilder();
            var config = new OrderConfiguration();

            modelBuilder.Entity<Order>(config.Configure);
            var entity = modelBuilder.Model.FindEntityType(typeof(Order));

            var orderDateProp = entity!.FindProperty(nameof(Order.OrderDate));
            var createdProp = entity.FindProperty(nameof(Order.CreatedOnTimeStamp));
            var modifiedProp = entity.FindProperty(nameof(Order.LastModifiedTimeStamp));

            Assert.NotNull(orderDateProp);
            Assert.False(orderDateProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, orderDateProp.GetColumnType());

            Assert.NotNull(createdProp);
            Assert.False(createdProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, createdProp.GetColumnType());

            Assert.NotNull(modifiedProp);
            Assert.False(modifiedProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, modifiedProp.GetColumnType());

            var fks = entity.GetForeignKeys().ToList();
            Assert.Equal(2, fks.Count);

            var customerFk = fks.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Customer));
            var operationSourceFk = fks.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(OperationSource));

            Assert.NotNull(customerFk);
            Assert.True(customerFk.IsRequired);
            Assert.Equal(nameof(Order.CustomerId), customerFk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, customerFk.DeleteBehavior);

            Assert.NotNull(operationSourceFk);
            Assert.False(operationSourceFk.IsRequired);
            Assert.Equal(nameof(Order.OperationSourceId), operationSourceFk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, operationSourceFk.DeleteBehavior);
        }
    }
}
