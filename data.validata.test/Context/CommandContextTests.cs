using data.validata.com.Context;
using Microsoft.EntityFrameworkCore;


namespace data.validata.test.Context
{
    public class CommandContextTests
    {
        [Fact]
        public void CommandContext_ShouldHaveDbSetsInitialized()
        {
            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase("CommandContextDb")
                .Options;

            using var context = new CommandContext(options);

            Assert.NotNull(context.Customer);
            Assert.NotNull(context.Order);
            Assert.NotNull(context.Product);
            Assert.NotNull(context.OrderItem);
        }

        [Fact]
        public void OnModelCreating_ShouldSetDefaultSchema()
        {
            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase("SchemaTestDb")
                .Options;

            using var context = new CommandContext(options);
            var model = context.Model;

            var schemaNames = model.GetEntityTypes()
                .Select(e => e.GetSchema())
                .Distinct()
                .ToList();

            Assert.All(schemaNames, s => Assert.Equal(DbConsts.DefaultSchema, s));
        }
    }
}
