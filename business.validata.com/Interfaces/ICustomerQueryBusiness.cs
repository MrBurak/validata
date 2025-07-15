using model.validata.com.Customer;
using model.validata.com;
using model.validata.com.Order;
using model.validata.com.DTO;


namespace business.validata.com.Interfaces
{
    public interface ICustomerQueryBusiness
    {
        Task<QueryResult<IEnumerable<CustomerDto>>> ListAsync(PaginationRequest paginationRequeste);

        Task<QueryResult<IEnumerable<OrderViewModel>>> ListOrderAsync(int customerId, PaginationRequest paginationRequeste);

        Task<QueryResult<CustomerDto?>> GetAsync(int id);



    }
}
