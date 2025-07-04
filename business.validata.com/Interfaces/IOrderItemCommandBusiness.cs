using data.validata.com.Entities;
namespace business.validata.com.Interfaces
{
    public interface IOrderItemCommandBusiness
    {
        Task<List<OrderItem>> AddAsync(Order order);
        Task DeleteAllAsync(int orderId);
        Task DeleteAllForCustomerAsync(int customerId);
    }
}
