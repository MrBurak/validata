using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<CustomerDto?> GetByIdAsync(int id);

        Task<IEnumerable<CustomerDto>> GetAllAsync(PaginationRequest paginationRequest);
    }
}
