using data.validata.com.Entities;
using model.validata.com;


namespace data.validata.com.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int ProductId);
        Task<IEnumerable<Product>> GetAllAsync(PaginationRequest paginationRequest);
        Task<IEnumerable<Product>> GetAllWithDeletedAsync();
    }
}
