using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Order;
using business.validata.com.Interfaces.Adaptors;
using model.validata.com.DTO;


namespace business.validata.com
{
    public class CustomerQueryBusiness : ICustomerQueryBusiness
    {
        private readonly ICustomerRepository repository;
        private readonly ILogger<CustomerQueryBusiness> logger;
        private readonly IOrderQueryBusiness orderQueryBusiness;
        private readonly ICustomerAdaptor customerAdaptor;
        public CustomerQueryBusiness(
            ICustomerRepository repository, 
            ILogger<CustomerQueryBusiness> logger, 
            IOrderQueryBusiness orderQueryBusiness,
            ICustomerAdaptor customerAdaptor)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(orderQueryBusiness);
            ArgumentNullException.ThrowIfNull(customerAdaptor);
            this.repository = repository;
            this.orderQueryBusiness = orderQueryBusiness;
            this.logger = logger;
            this.customerAdaptor = customerAdaptor;
        }
        public async Task<QueryResult<IEnumerable<CustomerDto>>> ListAsync(PaginationRequest paginationRequest)
        {
            logger.LogInformation("Fetching customers. PageNumber: {PageNumber}, PageSize: {PageSize}", paginationRequest.pageNumber, paginationRequest.pageSize);
            var queryResult = new QueryResult<IEnumerable<CustomerDto>>();

            try
            {
                var customers = await repository.GetAllAsync(paginationRequest);
                queryResult.Data = customers;
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

        public async Task<QueryResult<CustomerDto?>> GetAsync(int id)
        {
            logger.LogInformation("Fetching customer with ID: {CustomerId}", id);
            var queryResult = new QueryResult<CustomerDto?>();

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

                queryResult.Data = customer;
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
