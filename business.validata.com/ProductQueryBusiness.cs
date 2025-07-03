using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Product;
using Microsoft.Extensions.Logging;
using model.validata.com;
using util.validata.com;
using data.validata.com.Entities;


namespace business.validata.com
{
    public class ProductQueryBusiness : IProductQueryBusiness
    {
        private readonly IProductRepository repository;
        public ProductQueryBusiness(IProductRepository repository, ILogger<ProductQueryBusiness> logger)
        {
            ArgumentNullException.ThrowIfNull(repository);
            this.repository = repository;
        }
        public async Task<QueryResult<IEnumerable<ProductModel>>> ListAsync()
        {
            var queryResult = new QueryResult<IEnumerable<ProductModel>>();
            try
            {
                queryResult.Result= ObjectUtil.ConvertObj<IEnumerable<ProductModel>, IEnumerable<Product>>(await repository.GetAllAsync());
               
                queryResult.Success = true;
            }
            catch (Exception ex) 
            {
                queryResult.Success = false;
                queryResult.Exception=ex.Message;
            }
           return queryResult;
        }

        public async Task<QueryResult<ProductModel?>> GetAsync(int id)
        {
            var queryResult = new QueryResult<ProductModel?>();
            try 
            {
                var Product = (await repository.GetByIdAsync(id));
                if (Product == null) 
                {
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    return queryResult;
                }
                queryResult.Result = ObjectUtil.ConvertObj<ProductModel, Product>(Product);
                queryResult.Success = false;
            }
            catch (Exception ex)
            {
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;

        }
    }
}
