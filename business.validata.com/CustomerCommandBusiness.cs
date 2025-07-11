using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
using util.validata.com;

namespace business.validata.com
{
    public class CustomerCommandBusiness : ICustomerCommandBusiness
    {
        
        private readonly ICustomerValidation validation;
        private readonly IUnitOfWork unitOfWork;
        private readonly IOrderCommandBusiness orderCommandBusiness;
        private readonly IGenericLambdaExpressions genericLambdaExpressions;
        private readonly IGenericValidation<Customer> genericValidation;
        private readonly ILogger<CustomerCommandBusiness> logger;

        public CustomerCommandBusiness(
            ICustomerValidation validation,
            ICommandRepository<Customer> repository,
            IGenericValidation<Customer> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions,
            IOrderCommandBusiness orderCommandBusiness,
            IUnitOfWork unitOfWork,
            ILogger<CustomerCommandBusiness> logger) 
        {
            ArgumentNullException.ThrowIfNull(validation);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            ArgumentNullException.ThrowIfNull(orderCommandBusiness);
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(logger);
            this.validation = validation;
            this.unitOfWork = unitOfWork;
            this.orderCommandBusiness = orderCommandBusiness;
            this.genericLambdaExpressions = genericLambdaExpressions;
            this.genericValidation = genericValidation;
            this.logger = logger;
        }

        public async Task<CommandResult<CustomerViewModel>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation)
        {
            logger.LogInformation("Starting InvokeAsync for Customer with operation: {Operation}", businessSetOperation);

            CommandResult<CustomerViewModel> apiResult = new CommandResult<CustomerViewModel>();

            var validate = await validation.InvokeAsync(customer, businessSetOperation);
            if (!validate.IsValid)
            {
                logger.LogWarning("Validation failed for Customer invoke. Errors: {@Errors}", validate.Errors);
                apiResult.Validations = validate.Errors;
                return apiResult;
            }

            try
            {
                List<Action<Customer>> properties = new()
            {
                x =>
                {
                    x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                    x.OperationSourceId = (int) BusinessOperationSource.Api;
                    x.FirstName = customer.FirstName;
                    x.LastName = customer.LastName;
                    x.Pobox = customer.Pobox;
                    x.Address = customer.Address;
                }
            };

                Customer? result;

                if (businessSetOperation == BusinessSetOperation.Create)
                {
                    logger.LogInformation("Creating new Customer.");
                    result = await unitOfWork.customers.AddAsync(customer);
                    await unitOfWork.CommitAsync();
                    logger.LogInformation("Customer created with ID: {CustomerId}", result?.CustomerId);
                }
                else
                {
                    logger.LogInformation("Updating Customer with ID: {CustomerId}", customer.CustomerId);
                    var query = genericLambdaExpressions.GetEntityByPrimaryKey(customer);
                    await unitOfWork.customers.UpdateAsync(query, properties);
                    await unitOfWork.CommitAsync();
                    result = await unitOfWork.customers.GetEntityAsync(query);
                    logger.LogInformation("Customer updated with ID: {CustomerId}", customer.CustomerId);
                }

                apiResult.Data = ObjectUtil.ConvertObj<CustomerViewModel, Customer>(result!);
                apiResult.Success = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during InvokeAsync for Customer.");
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }

            return apiResult;
        }

        public async Task<CommandResult<Customer>> DeleteAsync(int id)
        {
            logger.LogInformation("Starting DeleteAsync for Customer with ID: {CustomerId}", id);

            CommandResult<Customer> apiResult = new CommandResult<Customer>();

            var exist = await genericValidation.Exists(id, BusinessSetOperation.Delete);
            if (exist != null && exist.Code != null)
            {
                logger.LogWarning("Delete validation failed for Customer ID: {CustomerId}. Validation code: {Code}", id, exist.Code);
                apiResult.Validations.Add(exist.Code);
                return apiResult;
            }

            List<Action<Customer>> properties = new()
        {
            x =>
            {
                x.DeletedOn = DateTimeUtil.SystemTime;
                x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                x.OperationSourceId = (int) BusinessOperationSource.Api;
            }
        };

            try
            {
                logger.LogInformation("Soft deleting Customer with ID: {CustomerId}", id);
                await unitOfWork.customers.UpdateAsync(genericLambdaExpressions.GetEntityById<Customer>(id), properties);
                await orderCommandBusiness.DeleteAllAsync(id);
                await unitOfWork.CommitAsync();
                apiResult.Success = true;
                logger.LogInformation("Customer with ID: {CustomerId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during DeleteAsync for Customer with ID: {CustomerId}", id);
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }

            return apiResult;
        }

    }
}
