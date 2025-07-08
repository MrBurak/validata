using Microsoft.EntityFrameworkCore;



using data.validata.com.Repositories;
using data.validata.com.Context;
using data.validata.com.Entities;

namespace data.validata.test.Repositories
{
    public class CommandRepositoryTests
    {
        private CommandContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            var context = new CommandContext(options);
            context.Database.EnsureDeleted();  
            context.Database.EnsureCreated();  
            return context;
        }

        [Fact]
        public void Constructor_InitializesDbContext()
        {
            using var context = GetInMemoryDbContext();
            var repository = new CommandRepository<Product>(context);
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task GetEntityAsync_ReturnsEntityAndDetachesIt()
        {
            
            using var context = GetInMemoryDbContext();
            var entityToAdd = new Product { ProductId = 0, Name = "Test1", Price = 0.00f };
            context.Product.Add(entityToAdd);
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            var retrievedEntity = await repository.GetEntityAsync(e => e.ProductId == 1);

            Assert.NotNull(retrievedEntity);
            Assert.Equal(1, retrievedEntity.ProductId);
            Assert.Equal("Test1", retrievedEntity.Name);

          
            var entryAfterFirstFetch = context.Entry(entityToAdd); 
            Assert.Equal(EntityState.Detached, entryAfterFirstFetch.State);
        }

        [Fact]
        public async Task GetEntityAsync_ReturnsNull_WhenEntityNotFound()
        {
            using var context = GetInMemoryDbContext();
            var repository = new CommandRepository<Product>(context);

            var retrievedEntity = await repository.GetEntityAsync(e => e.ProductId == 99);

            Assert.Null(retrievedEntity);
        }

        [Fact]
        public async Task GetListAsync_ReturnsListOfEntitiesAndDetachesThem()
        {
            
            using var context = GetInMemoryDbContext();
            context.Product.AddRange(
              new Product { ProductId = 1, Name = "ToDelete1", DeletedOn = null},
              new Product { ProductId = 2, Name = "ToDelete2", DeletedOn = DateTime.Now },
              new Product { ProductId = 3, Name = "ToKeep", DeletedOn = null}
            );
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            
            var retrievedEntities = await repository.GetListAsync(e => e.DeletedOn==null);

            
            Assert.NotNull(retrievedEntities);
            Assert.Equal(2, retrievedEntities.Count);
            Assert.Contains(retrievedEntities, e => e.ProductId == 1);
            Assert.Contains(retrievedEntities, e => e.ProductId == 3);

            foreach (var entity in context.Product.Local.ToList()) 
            {
                Assert.Equal(EntityState.Unchanged, context.Entry(entity).State);
            }
        }


        [Fact]
        public async Task GetListAsync_ReturnsEmptyList_WhenNoEntitiesFound()
        {
            
            using var context = GetInMemoryDbContext();
            var repository = new CommandRepository<Product>(context);

            
            var retrievedEntities = await repository.GetListAsync(e => e.Name == "NonExistent");

            
            Assert.NotNull(retrievedEntities);
            Assert.Empty(retrievedEntities);
        }

        [Fact]
        public async Task AddAsync_AddsEntityToContextAndSavesChanges()
        {
            
            using var context = GetInMemoryDbContext();
            var repository = new CommandRepository<Product>(context);
            var newEntity = new Product { ProductId = 4, Name = "NewTest", Price = 0f };

            
            var addedEntity = await repository.AddAsync(newEntity);

            
            Assert.NotNull(addedEntity);
            Assert.Equal(newEntity.ProductId, addedEntity.ProductId);
            Assert.Equal(newEntity.Name, addedEntity.Name);

            var savedEntity = await context.Product.FindAsync(4);
            Assert.NotNull(savedEntity);
            Assert.Equal("NewTest", savedEntity.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesEntitiesFromContextAndSavesChanges()
        {
            
            using var context = GetInMemoryDbContext();
            context.Product.AddRange(
                new Product { ProductId = 1, Name = "ToDelete1", DeletedOn=DateTime.Now},
                new Product { ProductId = 2, Name = "ToDelete2", DeletedOn=null },
                new Product { ProductId = 3, Name = "ToKeep", DeletedOn = DateTime.Now }
            );
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            
            var deleted = await repository.DeleteAsync(e => e.DeletedOn != null);

            
            Assert.True(deleted); 

            var remainingEntities = await context.Product.ToListAsync();
            Assert.Single(remainingEntities);
            Assert.DoesNotContain(remainingEntities, e => e.ProductId == 1 || e.ProductId == 3);  
            Assert.Contains(remainingEntities, e => e.ProductId == 2);  

        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNoEntitiesMatchQuery()
        {
            
            using var context = GetInMemoryDbContext();
            context.Product.Add(new Product { ProductId = 1, Name = "Test", Price=0f});
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            
            var deleted = await repository.DeleteAsync(e => e.Name == "NonExistent");

            
            Assert.False(deleted); 
            Assert.Equal(1, await context.Product.CountAsync());  
        }

        [Fact]
        public async Task UpdateAsync_UpdatesMatchingEntitiesAndSavesChanges()
        {
            
            using var context = GetInMemoryDbContext();
            context.Product.AddRange(
                new Product { ProductId = 1, Name = "OldName1", Price=0f},
                new Product { ProductId = 2, Name = "OldName2", Price=0f },
                new Product { ProductId = 3, Name = "OldName3", Price=0f}
            );
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            
            var propertiesToUpdate = new List<Action<Product>>
        {
            entity => entity.Name = "UpdatedName",
            entity => entity.Price=0f 
        };
            await repository.UpdateAsync(e => e.Name!.Contains("OldName"), propertiesToUpdate);

            
            var updatedEntity1 = await context.Product.FindAsync(1);
            Assert.NotNull(updatedEntity1);
            Assert.Equal("UpdatedName", updatedEntity1.Name);
            

            var updatedEntity2 = await context.Product.FindAsync(2);
            Assert.NotNull(updatedEntity2);
            Assert.Equal("UpdatedName", updatedEntity2.Name);  
            

            var updatedEntity3 = await context.Product.FindAsync(3);
            Assert.NotNull(updatedEntity3);
            Assert.Equal("UpdatedName", updatedEntity3.Name);
        }

        [Fact]
        public async Task UpdateAsync_NoEntitiesMatch_NoChangesMade()
        {
            
            using var context = GetInMemoryDbContext();
            context.Product.Add(new Product { ProductId = 1, Name = "Original", Price=0f});
            await context.SaveChangesAsync();

            var repository = new CommandRepository<Product>(context);

            
            var propertiesToUpdate = new List<Action<Product>>
        {
            entity => entity.Name = "NewName"
        };
            await repository.UpdateAsync(e => e.Name == "NonExistent", propertiesToUpdate);

            
            var entity = await context.Product.FindAsync(1);
            Assert.NotNull(entity);
            Assert.Equal("Original", entity.Name);  
        }
    }
}
