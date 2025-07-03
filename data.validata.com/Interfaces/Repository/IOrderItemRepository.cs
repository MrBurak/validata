using data.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(int orderItemId, int orderId);

        Task<IEnumerable<OrderItem>> GetAllAsync(int orderId);
    }
}
