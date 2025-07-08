using data.validata.com.Configuration;
using data.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using util.validata.com;

namespace data.validata.test.Entities.Configuration
{
    public class OrderItemConfigurationTests
    {
        [Fact]
        public void Configure_ShouldSetPropertiesAndRelationshipsCorrectly()
        {
            var modelBuilder = new ModelBuilder();
            var config = new OrderItemConfiguration();

            modelBuilder.Entity<OrderItem>(config.Configure);
            var entity = modelBuilder.Model.FindEntityType(typeof(OrderItem));

            var createdProp = entity!.FindProperty(nameof(OrderItem.CreatedOnTimeStamp));
            var modifiedProp = entity.FindProperty(nameof(OrderItem.LastModifiedTimeStamp));

            Assert.NotNull(createdProp);
            Assert.False(createdProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, createdProp.GetColumnType());

            Assert.NotNull(modifiedProp);
            Assert.False(modifiedProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, modifiedProp.GetColumnType());

            var fks = entity.GetForeignKeys().ToList();
            Assert.Equal(3, fks.Count);

            var productFk = fks.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Product));
            var orderFk = fks.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Order));
            var operationSourceFk = fks.FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(OperationSource));

            Assert.NotNull(productFk);
            Assert.True(productFk.IsRequired);
            Assert.Equal(nameof(OrderItem.ProductId), productFk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, productFk.DeleteBehavior);

            Assert.NotNull(orderFk);
            Assert.True(orderFk.IsRequired);
            Assert.Equal(nameof(OrderItem.OrderId), orderFk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, orderFk.DeleteBehavior);

            Assert.NotNull(operationSourceFk);
            Assert.False(operationSourceFk.IsRequired);
            Assert.Equal(nameof(OrderItem.OperationSourceId), operationSourceFk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, operationSourceFk.DeleteBehavior);
        }
    }
}
