using business.validata.com;
using data.validata.com.Context;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Repositories;
using Microsoft.EntityFrameworkCore;
using model.validata.com.Entities;
using model.validata.com.ValueObjects.Customer;

namespace business.validata.test
{


    public class UnitOfWorkTests
    {
        private readonly CommandContext context;
        private readonly ICommandRepository<Customer> customerRepo;
        private readonly ICommandRepository<Order> orderRepo;
        private readonly ICommandRepository<OrderItem> orderItemRepo;
        private readonly ICommandRepository<Product> productRepo;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<CommandContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            context = new CommandContext(options);
            productRepo = new CommandRepository<Product>(context);
            customerRepo = new CommandRepository<Customer>(context);
            orderRepo = new CommandRepository<Order>(context);
            orderItemRepo = new CommandRepository<OrderItem>(context);
        }

        private UnitOfWork CreateUnitOfWork() => new(
            context,
            customerRepo,
            orderRepo,
            orderItemRepo,
            productRepo
        );

        [Fact]
        public void Constructor_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(null!, customerRepo, orderRepo, orderItemRepo, productRepo));
        }

        [Fact]
        public void Constructor_WithNullCustomers_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(context, null!, orderRepo, orderItemRepo, productRepo));
        }

        [Fact]
        public void Constructor_WithNullOrders_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(context, customerRepo, null!, orderItemRepo, productRepo));
        }

        [Fact]
        public void Constructor_WithNullOrderItems_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(context, customerRepo, orderRepo, null!, productRepo));
        }

        [Fact]
        public void Constructor_WithNullProducts_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(context, customerRepo, orderRepo, orderItemRepo, null!));
        }

        [Fact]
        public async Task CommitAsync_ShouldCallSaveChangesAsync()
        {
            var customer = new Customer(0, new FirstName("John"), new LastName("Doe"), new EmailAddress("a@b.com"), new StreetAddress("a"), new PostalCode("a"));

            var uow = CreateUnitOfWork();
            await customerRepo.AddAsync(customer);
            var result = await uow.CommitAsync();
            Assert.Equal(1, result);
        }

        [Fact]
        public void Dispose_ShouldCallContextDispose()
        {
            var uow = CreateUnitOfWork();
            uow.Dispose();

        }
    }

}
