using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Customer;
using Microsoft.Extensions.Logging;
using model.validata.com;
using util.validata.com;
using data.validata.com.Entities;
using model.validata.com.Order;


namespace business.validata.com
{
    public class CustomerQueryBusiness : ICustomerQueryBusiness
    {
        private readonly ICustomerRepository repository;
        private readonly ILogger<CustomerQueryBusiness> logger;
        private readonly IOrderQueryBusiness orderQueryBusiness;
        public CustomerQueryBusiness(
            ICustomerRepository repository, 
            ILogger<CustomerQueryBusiness> logger, 
            IOrderQueryBusiness orderQueryBusiness)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(orderQueryBusiness);
            this.repository = repository;
            this.orderQueryBusiness = orderQueryBusiness;
            this.logger = logger;
        }
        public async Task<QueryResult<IEnumerable<CustomerViewModel>>> ListAsync(PaginationRequest paginationRequest)
        {
            logger.LogInformation("Fetching customers. PageNumber: {PageNumber}, PageSize: {PageSize}", paginationRequest.pageNumber, paginationRequest.pageSize);
            var queryResult = new QueryResult<IEnumerable<CustomerViewModel>>();

            try
            {
                var customers = await repository.GetAllAsync(paginationRequest);
                queryResult.Data = ObjectUtil.ConvertObj<IEnumerable<CustomerViewModel>, IEnumerable<Customer>>(customers);
                queryResult.Success = true;

                logger.LogInformation("Fetched {Count} customers successfully.", queryResult.Data?.Count() ?? 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch customers. PageNumber: {PageNumber}, PageSize: {PageSize}", paginationRequest.pageNumber, paginationRequest.pageSize);
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }

            return queryResult;
        }

        public async Task<QueryResult<CustomerViewModel?>> GetAsync(int id)
        {
            logger.LogInformation("Fetching customer with ID: {CustomerId}", id);
            var queryResult = new QueryResult<CustomerViewModel?>();

            try
            {
                var customer = await repository.GetByIdAsync(id);
                if (customer == null)
                {
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    logger.LogWarning("No customer found with ID: {CustomerId}", id);
                    return queryResult;
                }

                queryResult.Data = ObjectUtil.ConvertObj<CustomerViewModel, Customer>(customer);
                queryResult.Success = true;

                logger.LogInformation("Fetched customer with ID: {CustomerId} successfully.", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch customer with ID: {CustomerId}", id);
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }

            return queryResult;
        }

        public Task<QueryResult<IEnumerable<OrderViewModel>>> ListOrderAsync(int customerId, PaginationRequest paginationRequest)
        {
            logger.LogInformation("Delegating order list fetch for CustomerId: {CustomerId}", customerId);
            return orderQueryBusiness.ListAsync(customerId, paginationRequest);
        }
    }
}
