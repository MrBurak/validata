using data.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId, int customerId);

        Task<IEnumerable<Order>> GetAllAsync(int customerId);
    }
}
