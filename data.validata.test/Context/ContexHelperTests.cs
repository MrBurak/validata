using data.validata.com.Context;
using data.validata.com.Interfaces.Entities.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moq;
using System.Linq.Expressions;

namespace data.validata.test.Context
{
    public class ContexHelperTests
    {
        public class Product
        {
            public int Id { get; set; }
        }

        [Fact]
        public void Set_ShouldApplyConfigurationAndSeed()
        {
            
            var modelBuilder = new ModelBuilder();
            var configurationMock = new Mock<IEntityTypeConfiguration<Product>>();
            var seedMock = new Mock<IEntitySeed>();

            var helper = new ContexHelper<Product>(
                new DbContext(new DbContextOptions<DbContext>()),
                configurationMock.Object,
                "dbo",
                seedMock.Object
            );

            
            helper.Set(modelBuilder);

            
            configurationMock.Verify(c => c.Configure(It.IsAny<EntityTypeBuilder<Product>>()), Times.Once);
            seedMock.Verify(s => s.Invoke(It.IsAny<ModelBuilder>()), Times.Once);
        }

        [Fact]
        public void SetStandAlone_ShouldApplyConfigurationWithoutTemporal()
        {
            
            var modelBuilder = new ModelBuilder();
            var configurationMock = new Mock<IEntityTypeConfiguration<Product>>();
            var seedMock = new Mock<IEntitySeed>();

            var helper = new ContexHelper<Product>(
                new DbContext(new DbContextOptions<DbContext>()),
                configurationMock.Object,
                "dbo",
                seedMock.Object
            );

            
            helper.SetStandAlone(modelBuilder);

            
            configurationMock.Verify(c => c.Configure(It.IsAny<EntityTypeBuilder<Product>>()), Times.Once);
            seedMock.Verify(s => s.Invoke(It.IsAny<ModelBuilder>()), Times.Once);
        }

        [Fact]
        public void Set_ShouldSkipSeed_WhenSeedIsNull()
        {
            
            var modelBuilder = new ModelBuilder();
            var configurationMock = new Mock<IEntityTypeConfiguration<Product>>();

            var helper = new ContexHelper<Product>(
                new DbContext(new DbContextOptions<DbContext>()),
                configurationMock.Object,
                "dbo"
            );

            
            helper.Set(modelBuilder);

            
            configurationMock.Verify(c => c.Configure(It.IsAny<EntityTypeBuilder<Product>>()), Times.Once);
        }

        [Fact]
        public void Set_ShouldApplyIndexes()
        {
            
            var modelBuilder = new ModelBuilder();
            var configurationMock = new Mock<IEntityTypeConfiguration<Product>>();
            Expression<Func<Product, object>> expr = e => e.Id;

            var indexes = new List<TEntityIndex<Product>>
        {
            new TEntityIndex<Product>
            {
                Expression = expr!,
                IsUnique = true,
                Filter = null
            }
        };

            var helper = new ContexHelper<Product>(
                new DbContext(new DbContextOptions<DbContext>()),
                configurationMock.Object,
                "dbo"
            );

            
            helper.Set(modelBuilder, indexes);

            
            configurationMock.Verify(c => c.Configure(It.IsAny<EntityTypeBuilder<Product>>()), Times.Once);
        }
    }
}
