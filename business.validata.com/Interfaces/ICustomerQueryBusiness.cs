using model.validata.com.Customer;
using model.validata.com;


namespace business.validata.com.Interfaces
{
    public interface ICustomerQueryBusiness
    {
        Task<QueryResult<IEnumerable<CustomerViewModel>>> ListAsync();

        Task<QueryResult<CustomerViewModel?>> GetAsync(int id);

    }
}
