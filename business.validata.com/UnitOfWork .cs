using business.validata.com.Interfaces;
using data.validata.com.Context;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;


namespace business.validata.com
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommandContext context;

        public ICommandRepository<Customer> customers { get; }
        public ICommandRepository<Order> orders { get; }
        public ICommandRepository<OrderItem> orderItems { get; }
        public ICommandRepository<Product> products { get; }

        public UnitOfWork(
            CommandContext context,
            ICommandRepository<Customer> customers,
            ICommandRepository<Order> orders,
            ICommandRepository<OrderItem> orderItems,
            ICommandRepository<Product> products)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(customers);
            ArgumentNullException.ThrowIfNull(orders);
            ArgumentNullException.ThrowIfNull(orderItems);
            ArgumentNullException.ThrowIfNull(products);
            this.context = context;
            this.customers = customers;
            this.orders = orders;
            this.orderItems = orderItems;
            this.products = products;
        }

        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
