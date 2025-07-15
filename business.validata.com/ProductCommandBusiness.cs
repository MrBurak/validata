using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using model.validata.com.Entities;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using util.validata.com;
using business.validata.com.Adaptors;
using business.validata.com.Interfaces.Adaptors;

namespace business.validata.com
{
    public class ProductCommandBusiness : IProductCommandBusiness
    {

        private readonly IProductValidation validation;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericLambdaExpressions genericLambdaExpressions;
        private readonly IGenericValidation<Product> genericValidation;
        private readonly ILogger<ProductCommandBusiness> logger;
        private readonly IProductAdaptor productAdaptor;
        public ProductCommandBusiness(
            IProductValidation validation,
            IUnitOfWork unitOfWork,
            IGenericValidation<Product> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions,
            ILogger<ProductCommandBusiness> logger,
            IProductAdaptor productAdaptor
            )
        {
            ArgumentNullException.ThrowIfNull(validation);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(productAdaptor);
            this.validation = validation;
            this.unitOfWork = unitOfWork;
            this.genericLambdaExpressions = genericLambdaExpressions;
            this.genericValidation = genericValidation;
            this.logger = logger;
            this.productAdaptor = productAdaptor;
        }

        public async Task<CommandResult<ProductModel>> InvokeAsync(Product product, BusinessSetOperation businessSetOperation)
        {
            logger.LogInformation("Invoking product {Operation} operation for product with ID: {ProductId}", businessSetOperation, product.ProductId);

            CommandResult<ProductModel> apiResult = new CommandResult<ProductModel>();
            var validate = await validation.InvokeAsync(product, businessSetOperation);
            if (!validate.IsValid)
            {
                logger.LogWarning("Validation failed for product {ProductId} with errors: {Errors}", product.ProductId, validate.Errors);
                apiResult.Validations = validate.Errors;
                return apiResult;
            }

            try
            {
                List<Action<Product>> properties = new()
            {
                x =>
                {
                    x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                    x.OperationSourceId = (int)BusinessOperationSource.Api;
                    x.ChangeName(product.Name);
                    x.UpdatePrice(product.Price);
                }
            };
                product.OperationSourceId = (int)BusinessOperationSource.Api;

                Product? result;
                if (businessSetOperation == BusinessSetOperation.Create)
                {
                    logger.LogInformation("Creating new product: {ProductName}", product.Name);
                    result = await unitOfWork.products.AddAsync(product);
                    await unitOfWork.CommitAsync();
                    logger.LogInformation("Product created with ID: {ProductId}", result?.ProductId);
                }
                else
                {
                    var query = genericLambdaExpressions.GetEntityByPrimaryKey(product);
                    logger.LogInformation("Updating product with ID: {ProductId}", product.ProductId);
                    await unitOfWork.products.UpdateAsync(query, properties);
                    await unitOfWork.CommitAsync();
                    result = await unitOfWork.products.GetEntityAsync(query);
                    logger.LogInformation("Product updated with ID: {ProductId}", result?.ProductId);
                }

                apiResult.Data = productAdaptor.Invoke(result!);
                apiResult.Success = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while processing product {ProductId}", product.ProductId);
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }

            return apiResult;
        }

        public async Task<CommandResult<Product>> DeleteAsync(int id)
        {
            logger.LogInformation("Deleting product with ID: {ProductId}", id);

            CommandResult<Product> apiResult = new CommandResult<Product>();
            var exist = await genericValidation.Exists(id, BusinessSetOperation.Delete);
            if (exist != null && exist.Code != null)
            {
                logger.LogWarning("Product deletion failed for ID: {ProductId} due to validation: {ValidationCode}", id, exist.Code);
                apiResult.Validations.Add(exist.Code);
                return apiResult;
            }

            

            try
            {
                await unitOfWork.products.DeleteAsync(genericLambdaExpressions.GetEntityById<Product>(id));
                await unitOfWork.CommitAsync();
                apiResult.Success = true;
                logger.LogInformation("Product deleted with ID: {ProductId}", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while deleting product with ID: {ProductId}", id);
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }

            return apiResult;
        }
    }
}
