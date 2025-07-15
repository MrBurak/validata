using data.validata.com.Entities;
using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface IOrderRepository
    {
        Task<OrderDto?> GetByIdAsync(int orderId, int customerId);

        Task<IEnumerable<OrderDto>> GetAllAsync(int customerId, PaginationRequest paginationRequest);
    }
}
