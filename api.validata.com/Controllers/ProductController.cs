using business.validata.com.Interfaces;
using model.validata.com.Product;
using model.validata.com.Entities;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;
using business.validata.com.Interfaces.Adaptors;


namespace api.validata.com.Controllers
{

    /// <summary>
    /// Controller for managing product-related operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
       

        private readonly ILogger<ProductController> logger;

        private readonly IProductCommandBusiness commandBusiness;
        private readonly IProductQueryBusiness queryBusiness;
        private readonly IProductAdaptor adaptor;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance used for recording operational logs.</param>
        /// <param name="commandBusiness">Service for handling product commands (insert, update, delete).</param>
        /// <param name="queryBusiness">Service for querying product information.</param>
        /// <param name="adaptor">Adaptor for mapping product models.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        
        public ProductController(ILogger<ProductController> logger, IProductCommandBusiness commandBusiness, IProductQueryBusiness queryBusiness, IProductAdaptor adaptor)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            ArgumentNullException.ThrowIfNull(adaptor);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;   
            this.adaptor = adaptor;

           
        }

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of products per page.</param>
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing a list of <see cref="ProductModel"/> instances.
        /// </returns>
        [HttpGet("list/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<ProductModel>>> List(int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching product list. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

            var result = await queryBusiness.ListAsync(new PaginationRequest(pageNumber, pageSize));

            logger.LogInformation("Product list fetched. Count: {Count}, PageNumber: {PageNumber}", result.Data?.Count() ?? 0, pageNumber);

            return result;
        }

        /// <summary>
        /// Inserts a new product.
        /// </summary>
        /// <param name="request">The product data to be inserted.</param>
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the inserted <see cref="ProductModel"/>.
        /// </returns>
        [HttpPost("insert")]
        public async Task<CommandResult<ProductModel>> Insert(ProductBaseModel request)
        {

            var product = adaptor.Invoke(new ProductModel { ProductId = 0, Name = request.Name, Price = request.Price });
            var result = await commandBusiness.InvokeAsync(product, BusinessSetOperation.Create);
            logger.LogInformation("Product inserted. Success: {Success}", result.Success);
            return result;

        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="request">The updated product data.</param>
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the updated <see cref="ProductModel"/>.
        /// </returns>
        [HttpPut("update")]
        public async Task<CommandResult<ProductModel>> Update(ProductModel request)
        {
            logger.LogInformation("Updating product. ID: {Id}", request.ProductId);
            var product = adaptor.Invoke(request);
            var result = await commandBusiness.InvokeAsync(product, BusinessSetOperation.Update);
            logger.LogInformation("Product update completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the deleted <see cref="Product"/> entity.
        /// </returns>
        [HttpDelete("delete")]
        public async Task<CommandResult<Product>> Delete(int id)
        {
            logger.LogWarning("Deleting product. ID: {Id}", id);
            var result = await commandBusiness.DeleteAsync(id);
            logger.LogInformation("Product deletion completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing the <see cref="ProductModel"/> if found; otherwise, null.
        /// </returns>
        [HttpGet("get/{id}")]
        public async Task<QueryResult<ProductModel?>> Get(int id)
        {
            logger.LogInformation("Fetching product by ID: {Id}", id);
            var result = await queryBusiness.GetAsync(id);
            logger.LogInformation("Product fetch completed. Found: {Found}", result.Data != null);
            return result;
        }
    }
}
