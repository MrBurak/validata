using data.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);

        Task<IEnumerable<Customer>> GetAllAsync();
    }
}
