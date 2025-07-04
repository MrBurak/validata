using data.validata.com.Entities;


namespace data.validata.com.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int ProductId);

        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllWithDeletedAsync();
    }
}
