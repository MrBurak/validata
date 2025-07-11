using data.validata.com.Entities;
using model.validata.com;


namespace data.validata.com.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);

        Task<IEnumerable<Customer>> GetAllAsync(PaginationRequest paginationRequest);
    }
}
