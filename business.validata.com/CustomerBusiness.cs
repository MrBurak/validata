using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using util.validata.com;

namespace business.validata.com
{
    public class CustomerBusiness : AbstractBusiness<Customer>, ICustomerBusiness
    {
        
        private readonly ICustomerValidation validation;
        public CustomerBusiness(
            ICustomerValidation validation,
            IDataRepository<Customer> repository,
            IGenericValidation<Customer> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions) :
            base(genericValidation, repository, genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(validation);
            this.validation = validation;
        }

        public async Task<ApiResult<Customer>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation) 
        {
            ApiResult<Customer> apiResult = new ApiResult<Customer>();
            var validate = await validation.InvokeAsync(customer, businessSetOperation);
            if (!validate.IsValid) 
            {
                apiResult.Validations=validate.Errors;
                return apiResult;
            }
            try
            {
                List<Action<Customer>> properties = new()
                {
                    x=>
                    {
                        x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                        x.OperationSourceId = (int) BusinessOperationSource.Api;
                        x.FirstName=customer.FirstName;
                        x.LastName=customer.LastName;
                        x.Pobox=customer.Pobox;
                        x.Address=customer.Address;
                    }
                };
                apiResult.Result = await BaseInvokeAsync(validate.Entity!, customer, businessSetOperation, properties);
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
