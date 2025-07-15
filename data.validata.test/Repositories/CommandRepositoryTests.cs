using data.validata.com.Context;
using data.validata.com.Repositories;
using Microsoft.EntityFrameworkCore;
using model.validata.com.Entities;
using model.validata.com.ValueObjects.Product;


namespace data.validata.test.Repositories
{

    public class CommandRepositoryTests
    {
        private readonly CommandContext _context;
        private readonly CommandRepository<Product> _repository;

        public CommandRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new CommandContext(options);
            _repository = new CommandRepository<Product>(_context);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            var entity = new Product(1,new ProductName("Test Entity"), new ProductPrice(0));

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync();

            var saved = await _context.Set<Product>().FindAsync(1);

            Assert.NotNull(saved);
            Assert.Equal("Test Entity", saved?.Name!);
        }

        [Fact]
        public async Task GetEntityAsync_Should_Return_Matching_Entity()
        {
            var entity = new Product(2, new ProductName("FindMe"), new ProductPrice(0));
            await _context.Set<Product>().AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _repository.GetEntityAsync(x => x.Name == "FindMe");

            Assert.NotNull(result);
            Assert.Equal("FindMe", result?.Name!);
        }

        [Fact]
        public async Task GetListAsync_Should_Return_Entities()
        {
            var entity1 = new Product(3, new ProductName("E1"), new ProductPrice(0));
            var entity2 = new Product(4, new ProductName("E2"), new ProductPrice(0));

            await _context.Set<Product>().AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetListAsync(x => x.ProductId > 0);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteAsync_Should_SoftDelete_Entities()
        {
            var entity = new Product(5, new ProductName("ToDelete"), new ProductPrice(0));
            await _context.Set<Product>().AddAsync(entity);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(x => x.Name == "ToDelete");
            await _context.SaveChangesAsync();

            var deleted = await _context.Set<Product>().FirstOrDefaultAsync(x => x.ProductId == 5);
            Assert.NotNull(deleted?.DeletedOn);
        }

        [Fact]
        public async Task UpdateAsync_Should_Apply_Changes()
        {
            var entity = new Product(6, new ProductName("Before"), new ProductPrice(0));
            await _context.Set<Product>().AddAsync(entity);
            await _context.SaveChangesAsync();

            await _repository.UpdateAsync(x => x.ProductId == 6, new List<Action<Product>>
            {
                x => x.ChangeName(new ProductName("After"))
            });
            await _context.SaveChangesAsync();

            var updated = await _context.Set<Product>().FindAsync(6);
            Assert.Equal("After", updated?.Name!);
        }

        
    }

}
