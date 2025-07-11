

using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;

namespace business.validata.com.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICommandRepository<Customer> customers { get; }
        ICommandRepository<Order> orders { get; }
        ICommandRepository<Product> products { get; }
        ICommandRepository<OrderItem> orderItems { get; }

        Task<int> CommitAsync();
    }
}
