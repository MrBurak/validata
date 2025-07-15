using business.validata.com.Interfaces;
using model.validata.com.Order;
using model.validata.com.Entities;
using Microsoft.AspNetCore.Mvc;
using model.validata.com;
using model.validata.com.Enumeration;


namespace api.validata.com.Controllers
{
    /// <summary>
    /// Controller responsible for managing customer orders.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
       

        private readonly ILogger<OrderController> logger;

        private readonly IOrderCommandBusiness commandBusiness;
        private readonly IOrderQueryBusiness queryBusiness;


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for recording operational events.</param>
        /// <param name="queryBusiness">Service for querying order data.</param>
        /// <param name="commandBusiness">Service for handling order commands such as insert, update, and delete.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
        public OrderController(ILogger<OrderController> logger, IOrderQueryBusiness queryBusiness, IOrderCommandBusiness commandBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;

          
        }
        /// <summary>
        /// Retrieves a paginated list of orders for a specific customer.
        /// </summary>
        /// <param name="customerid">The ID of the customer.</param>
        /// <param name="pageNumber">The current page number (starting from 1).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A <see cref="QueryResult{T}"/> containing a list of <see cref="OrderViewModel"/> instances.</returns>

        [HttpGet("list/{customerid}/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> List(int customerid, int pageNumber, int pageSize)
        {
            logger.LogInformation("Retrieving order list for customer ID: {CustomerId}", customerid);
            var result = await queryBusiness.ListAsync(customerid, new PaginationRequest(pageNumber, pageSize));
            logger.LogInformation("Retrieved {Count} orders for customer ID: {CustomerId}", result.Data?.Count() ?? 0, customerid);
            return result;
        }

        /// <summary>
        /// Creates a new order for a customer.
        /// </summary>
        /// <param name="request">The order data to be inserted.</param>
        /// <returns>A <see cref="CommandResult{T}"/> containing the created <see cref="OrderDetailViewModel"/>.</returns>

        [HttpPost("insert")]
        public async Task<CommandResult<OrderDetailViewModel>> Insert(OrderInsertModel request)
        {
            logger.LogInformation("Inserting a new order for customer ID: {CustomerId}", request.CustomerId);
            var order = new OrderUpdateModel { CustomerId = request.CustomerId, Items=request.Items };
            var result = await commandBusiness.InvokeAsync(order, BusinessSetOperation.Create);
            logger.LogInformation("Order insertion completed. Success: {Success}", result.Success);
            return result;

        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="request">The updated order data.</param>
        /// <returns>A <see cref="CommandResult{T}"/> containing the updated <see cref="OrderDetailViewModel"/>.</returns>

        [HttpPut("update")]
        public async Task<CommandResult<OrderDetailViewModel>> Update(OrderUpdateModel request)
        {
            logger.LogInformation("Updating order ID: {OrderId}", request.OrderId);
            var result = await commandBusiness.InvokeAsync(request, BusinessSetOperation.Update);
            logger.LogInformation("Order update completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>A <see cref="CommandResult{T}"/> indicating the result of the delete operation.</returns>

        [HttpDelete("delete")]
        public async Task<CommandResult<Order>> Delete(int id)
        {
            logger.LogWarning("Deleting order with ID: {OrderId}", id);
            var result = await commandBusiness.DeleteAsync(id);
            logger.LogInformation("Order deletion completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Retrieves a specific order by ID for a given customer.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="customerId">The ID of the customer who owns the order.</param>
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing the <see cref="OrderDetailViewModel"/> if found; otherwise, null.
        /// </returns>
        [HttpGet("get/{orderId}/{customerId}")]
        public async Task<QueryResult<OrderDetailViewModel?>> Get(int orderId, int customerId)
        {
            logger.LogInformation("Retrieving order with ID: {OrderId} for customer ID: {CustomerId}", orderId, customerId);
            var result = await queryBusiness.GetAsync(orderId, customerId);
            logger.LogInformation("Order retrieval completed. Found: {Found}", result.Data != null);
            return result;
        }
    }
}
