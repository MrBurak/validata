using business.validata.com.Interfaces;
using model.validata.com.Customer;
using model.validata.com.Entities;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using business.validata.com.Interfaces.Adaptors;
using model.validata.com.DTO;


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
        private readonly ICustomerAdaptor adaptor;


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="commandBusiness">The command business logic for customers.</param>
        /// <param name="queryBusiness">The query business logic for customers.</param>
        /// <param name="adaptor">The adaptor for converting between models and domain entities.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the dependencies are null.</exception>
        public CustomerController(ILogger<CustomerController> logger, ICustomerCommandBusiness commandBusiness, ICustomerQueryBusiness queryBusiness, ICustomerAdaptor adaptor)
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
        /// Retrieves a paginated list of customers.
        /// </summary>
        /// <param name="pageNumber">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of customers per page.</param>
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing a list of <see cref="CustomerDto"/> instances.
        /// </returns>
        [HttpGet("list/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<CustomerDto>>> List(int pageNumber, int pageSize)
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
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the inserted <see cref="CustomerDto"/>.
        /// </returns>
        [HttpPost("insert")]
        public async Task<CommandResult<CustomerDto>> Insert(CustomerInsertModel request)
        {
            logger.LogInformation("Inserting a new customer: {Email}", request.Email);
            var customer =  adaptor.Invoke(request);
            var result = await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Create);
            logger.LogInformation("Customer inserted. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="request">The customer update model containing the updated data.</param>
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the updated <see cref="CustomerDto"/>.
        /// </returns>
        [HttpPut("update")]
        public async Task<CommandResult<CustomerDto>> Update(CustomerUpdateModel request)
        {
            logger.LogInformation("Updating customer ID: {CustomerId}", request.CustomerId);
            var customer = adaptor.Invoke(request);
            var result = await commandBusiness.InvokeAsync(customer, BusinessSetOperation.Update);
            logger.LogInformation("Customer update completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Deletes a customer by ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to delete.</param>
        /// <returns>
        /// A <see cref="CommandResult{T}"/> containing the deleted <see cref="Customer"/> entity.
        /// </returns>

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
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing the <see cref="CustomerDto"/> if found; otherwise, null.
        /// </returns>
        [HttpGet("get/{customerId}")]
        public async Task<QueryResult<CustomerDto?>> Get(int customerId)
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
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing a list of <see cref="OrderViewModel"/> instances.
        /// </returns>
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
