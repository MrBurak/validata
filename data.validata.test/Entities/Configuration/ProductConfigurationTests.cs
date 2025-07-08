using data.validata.com.Configuration;
using data.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using util.validata.com;

namespace data.validata.test.Entities.Configuration
{
    public class ProductConfigurationTests
    {
        [Fact]
        public void Configure_ShouldSetPropertiesAndRelationshipsCorrectly()
        {
            var modelBuilder = new ModelBuilder();
            var config = new ProductConfiguration();

            modelBuilder.Entity<Product>(config.Configure);
            var entity = modelBuilder.Model.FindEntityType(typeof(Product));

            var nameProp = entity!.FindProperty(nameof(Product.Name));
            var createdProp = entity.FindProperty(nameof(Product.CreatedOnTimeStamp));
            var modifiedProp = entity.FindProperty(nameof(Product.LastModifiedTimeStamp));

            Assert.NotNull(nameProp);
            Assert.False(nameProp.IsNullable);
            Assert.Equal(128, nameProp.GetMaxLength());
            Assert.Equal("nvarchar", nameProp.GetColumnType());

            Assert.NotNull(createdProp);
            Assert.False(createdProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, createdProp.GetColumnType());

            Assert.NotNull(modifiedProp);
            Assert.False(modifiedProp.IsNullable);
            Assert.Equal(DateTimeUtil.DbDate_DataType, modifiedProp.GetColumnType());

            var fk = entity.GetForeignKeys().FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(OperationSource));

            Assert.NotNull(fk);
            Assert.False(fk.IsRequired);
            Assert.Equal(nameof(Product.OperationSourceId), fk.Properties.First().Name);
            Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
        }
    }
}
