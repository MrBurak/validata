using data.validata.com.Entities;
using model.validata.com;


namespace data.validata.com.Interfaces.Repository
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId, int customerId);

        Task<IEnumerable<Order>> GetAllAsync(int customerId, PaginationRequest paginationRequest);
    }
}
