using AutoMapper;
using business.validata.com.Interfaces;
using model.validata.com.Customer;
using data.validata.com.Entities;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;


namespace api.validata.com.Controllers
{
    /// <summary>
    /// Controller for managing customer-related operations.
    /// </summary>

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
       

        private readonly ILogger<CustomerController> logger;
        private readonly ICustomerCommandBusiness commandBusiness;
        private readonly ICustomerQueryBusiness queryBusiness;
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="commandBusiness">The command business logic for customers.</param>
        /// <param name="queryBusiness">The query business logic for customers.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the dependencies are null.</exception>
        public CustomerController(ILogger<CustomerController> logger, ICustomerCommandBusiness commandBusiness, ICustomerQueryBusiness queryBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;   

            mapperConfiguration= new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<CustomerInsertModel, Customer>();
                mapperConfigurationExpression.CreateMap<CustomerUpdateModel, Customer>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        /// <summary>
        /// Retrieves a paginated list of customers.
        /// </summary>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of customers per page.</param>
        /// <returns>A <see cref="QueryResult{T}"/> containing a list of <see cref="CustomerViewModel"/> instances.</returns>
        [HttpGet("list/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<CustomerViewModel>>> List(int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching customers. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

            var result = await queryBusiness.ListAsync(new PaginationRequest(pageNumber, pageSize));

            logger.LogInformation("Fetched {Count} customers for page {PageNumber}.", result.Data?.Count() ?? 0, pageNumber);

            return result;
        }

        /// <summary>
        /// Inserts a new customer.
        /// </summary>
        /// <param name="request">The customer insert model containing the new customer's data.</param>
        /// <returns>The result of the create operation including the inserted customer view model.</returns>
        [HttpPost("insert")]
        public async Task<CommandResult<CustomerViewModel>> Insert(CustomerInsertModel request)
        {
            logger.LogInformation("Inserting a new customer: {Email}", request.Email);
            var customer = mapper.Map<Customer>(request);
            var result = await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Create);
            logger.LogInformation("Customer inserted. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="request">The customer update model containing the updated data.</param>
        /// <returns>The result of the update operation including the updated customer view model.</returns>
        [HttpPut("update")]
        public async Task<CommandResult<CustomerViewModel>> Update(CustomerUpdateModel request)
        {
            logger.LogInformation("Updating customer ID: {CustomerId}", request.CustomerId);
            var customer = mapper.Map<Customer>(request);
            var result = await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Update);
            logger.LogInformation("Customer update completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to delete.</param>
        /// <returns>The result of the delete operation including the deleted customer entity.</returns>
        [HttpDelete("delete")]
        public async Task<CommandResult<Customer>> Delete(int customerId)
        {
            logger.LogWarning("Deleting customer ID: {CustomerId}", customerId);
            var result = await commandBusiness.DeleteAsync(customerId);
            logger.LogInformation("Customer deletion completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Retrieves a specific customer by ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve.</param>
        /// <returns>The query result containing the customer view model if found.</returns>
        [HttpGet("get/{customerId}")]
        public async Task<QueryResult<CustomerViewModel?>> Get(int customerId)
        {
            logger.LogInformation("Fetching customer by ID: {CustomerId}", customerId);
            var result = await queryBusiness.GetAsync(customerId);
            logger.LogInformation("Customer fetch completed. Found: {Found}", result.Data != null);
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of orders for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer whose orders are to be retrieved.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>A query result containing a list of order view models.</returns>
        [HttpGet("getorders/{customerId}/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> ListOrders(int customerId, int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching orders for CustomerId={CustomerId}, PageNumber={PageNumber}, PageSize={PageSize}", customerId, pageNumber, pageSize);

            var result = await queryBusiness.ListOrderAsync(customerId, new PaginationRequest(pageNumber, pageSize));

            logger.LogInformation("Fetched {Count} orders for CustomerId={CustomerId}, PageNumber={PageNumber}", result.Data?.Count() ?? 0, customerId, pageNumber);

            return result;
        }
    }
}
