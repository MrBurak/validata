using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
using util.validata.com;

namespace business.validata.com
{
    public class CustomerCommandBusiness : AbstractCommandBusiness<Customer>, ICustomerCommandBusiness
    {
        
        private readonly ICustomerValidation validation;
        public CustomerCommandBusiness(
            ICustomerValidation validation,
            ICommandRepository<Customer> repository,
            IGenericValidation<Customer> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions) :
            base(genericValidation, repository, genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(validation);
            this.validation = validation;
        }

        public async Task<CommandResult<CustomerViewModel>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation) 
        {
            CommandResult<CustomerViewModel> apiResult = new CommandResult<CustomerViewModel>();
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
                var result= await BaseInvokeAsync(validate.Entity!, customer, businessSetOperation, properties);
                apiResult.Result = ObjectUtil.ConvertObj<CustomerViewModel, Customer>(result!);
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
