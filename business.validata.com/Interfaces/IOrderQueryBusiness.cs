using model.validata.com;
using model.validata.com.Order;


namespace business.validata.com.Interfaces
{
    public interface IOrderQueryBusiness
    {
        Task<QueryResult<IEnumerable<OrderViewModel>>> ListAsync(int customerId, PaginationRequest paginationRequest);

        Task<QueryResult<OrderDetailViewModel?>> GetAsync(int orderId, int customerId);
    }
}
