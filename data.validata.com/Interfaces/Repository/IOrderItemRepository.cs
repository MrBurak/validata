using model.validata.com.DTO;


namespace data.validata.com.Interfaces.Repository
{
    public interface IOrderItemRepository
    {
        Task<OrderItemDto?> GetByIdAsync(int orderItemId, int orderId);

        Task<IEnumerable<OrderItemDto>> GetAllAsync(int orderId);
    }
}
