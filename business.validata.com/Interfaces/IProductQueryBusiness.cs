using model.validata.com.Product;
using model.validata.com;


namespace business.validata.com.Interfaces
{
    public interface IProductQueryBusiness
    {
        Task<QueryResult<IEnumerable<ProductModel>>> ListAsync();

        Task<QueryResult<ProductModel?>> GetAsync(int id);
    }
}
