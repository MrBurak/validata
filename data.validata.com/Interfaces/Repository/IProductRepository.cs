using model.validata.com;
using model.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int ProductId);
        Task<IEnumerable<Product>> GetAllAsync(PaginationRequest paginationRequest);
        Task<IEnumerable<Product>> GetAllWithDeletedAsync();
    }
}
