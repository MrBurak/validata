using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using util.validata.com;

namespace business.validata.com
{
    public class ProductCommandBusiness : AbstractCommandBusiness<Product>, IProductCommandBusiness
    {
        
        private readonly IProductValidation validation;
        public ProductCommandBusiness(
            IProductValidation validation,
            ICommandRepository<Product> repository,
            IGenericValidation<Product> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions) :
            base(genericValidation, repository, genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(validation);
            this.validation = validation;
        }

        public async Task<CommandResult<ProductModel>> InvokeAsync(Product Product, BusinessSetOperation businessSetOperation) 
        {
            CommandResult<ProductModel> apiResult = new CommandResult<ProductModel>();
            var validate = await validation.InvokeAsync(Product, businessSetOperation);
            if (!validate.IsValid) 
            {
                apiResult.Validations=validate.Errors;
                return apiResult;
            }
            try
            {
                List<Action<Product>> properties = new()
                {
                    x=>
                    {
                        x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                        x.OperationSourceId = (int) BusinessOperationSource.Api;
                        x.Name=Product.Name;
                        x.Price=Product.Price;
                    }
                };
                var result= await BaseInvokeAsync(validate.Entity!, Product, businessSetOperation, properties);
                apiResult.Result = ObjectUtil.ConvertObj<ProductModel, Product>(result!);
                apiResult.Success = true;

            }
            catch (Exception ex)
            {
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }
            return apiResult;
        }            
            
        }
    }
