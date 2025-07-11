using model.validata.com.Customer;
using model.validata.com;
using model.validata.com.Order;


namespace business.validata.com.Interfaces
{
    public interface ICustomerQueryBusiness
    {
        Task<QueryResult<IEnumerable<CustomerViewModel>>> ListAsync(PaginationRequest paginationRequeste);

        Task<QueryResult<IEnumerable<OrderViewModel>>> ListOrderAsync(int customerId, PaginationRequest paginationRequeste);

        Task<QueryResult<CustomerViewModel?>> GetAsync(int id);



    }
}
