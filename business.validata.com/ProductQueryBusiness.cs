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
        private readonly ILogger<ProductQueryBusiness> logger;
        public ProductQueryBusiness(IProductRepository repository, ILogger<ProductQueryBusiness> logger)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(logger);
            this.repository = repository;
            this.logger = logger;
        }
        public async Task<QueryResult<IEnumerable<ProductModel>>> ListAsync(PaginationRequest paginationRequest)
        {
            logger.LogInformation("Listing products, PageNumber={PageNumber}, PageSize={PageSize}", paginationRequest.pageNumber, paginationRequest.pageSize);
            var queryResult = new QueryResult<IEnumerable<ProductModel>>();
            try
            {
                var products = await repository.GetAllAsync(paginationRequest);
                queryResult.Data = ObjectUtil.ConvertObj<IEnumerable<ProductModel>, IEnumerable<Product>>(products);
                queryResult.Success = true;
                logger.LogInformation("Listed {Count} products.", queryResult.Data?.Count() ?? 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while listing products");
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;
        }

        public async Task<QueryResult<ProductModel?>> GetAsync(int id)
        {
            logger.LogInformation("Getting product details for ProductId={ProductId}", id);
            var queryResult = new QueryResult<ProductModel?>();
            try
            {
                var product = await repository.GetByIdAsync(id);
                if (product == null)
                {
                    logger.LogWarning("Product not found with ID={ProductId}", id);
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    return queryResult;
                }
                queryResult.Data = ObjectUtil.ConvertObj<ProductModel, Product>(product);
                queryResult.Success = true;
                logger.LogInformation("Product details retrieved for ProductId={ProductId}", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting product details for ProductId={ProductId}", id);
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;
        }
    }
}
