using AutoMapper;
using business.validata.com.Interfaces;
using model.validata.com.Order;
using data.validata.com.Entities;
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
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging information and errors.</param>
        /// <param name="queryBusiness">The order query business layer.</param>
        /// <param name="commandBusiness">The order command business layer.</param>
        public OrderController(ILogger<OrderController> logger, IOrderQueryBusiness queryBusiness, IOrderCommandBusiness commandBusiness)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(commandBusiness);
            ArgumentNullException.ThrowIfNull(queryBusiness);
            this.logger = logger;
            this.commandBusiness = commandBusiness;
            this.queryBusiness = queryBusiness;

            mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.CreateMap<OrderInsertModel, OrderUpdateModel>();

            });
            mapper = mapperConfiguration.CreateMapper();
        }
        /// <summary>
        /// Retrieves the list of orders for a specific customer.
        /// </summary>
        /// <param name="customerid">The ID of the customer whose orders are to be retrieved.</param>
        /// <returns>A query result containing a list of order view models.</returns>
        [HttpGet("list/{customerid}/{pageNumber}/{pageSize}")]
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> List(int customerid, int pageNumber, int pageSize)
        {
            logger.LogInformation("Retrieving order list for customer ID: {CustomerId}", customerid);
            var result = await queryBusiness.ListAsync(customerid, new PaginationRequest(pageNumber, pageSize));
            logger.LogInformation("Retrieved {Count} orders for customer ID: {CustomerId}", result.Data?.Count() ?? 0, customerid);
            return result;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">The order data to insert.</param>
        /// <returns>A command result containing the details of the created order.</returns>
        [HttpPost("insert")]
        public async Task<CommandResult<OrderDetailViewModel>> Insert(OrderInsertModel request)
        {
            logger.LogInformation("Inserting a new order for customer ID: {CustomerId}", request.CustomerId);
            var order = mapper.Map<OrderUpdateModel>(request);
            var result = await commandBusiness.InvokeAsync(order, BusinessSetOperation.Create);
            logger.LogInformation("Order insertion completed. Success: {Success}", result.Success);
            return result;

        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="request">The updated order data.</param>
        /// <returns>A command result containing the updated order details.</returns>
        [HttpPut("update")]
        public async Task<CommandResult<OrderDetailViewModel>> Update(OrderUpdateModel request)
        {
            logger.LogInformation("Updating order ID: {OrderId}", request.OrderId);
            var result = await commandBusiness.InvokeAsync(request, BusinessSetOperation.Update);
            logger.LogInformation("Order update completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>A command result indicating the outcome of the delete operation.</returns>
        [HttpDelete("delete")]
        public async Task<CommandResult<Order>> Delete(int id)
        {
            logger.LogWarning("Deleting order with ID: {OrderId}", id);
            var result = await commandBusiness.DeleteAsync(id);
            logger.LogInformation("Order deletion completed. Success: {Success}", result.Success);
            return result;
        }

        /// <summary>
        /// Retrieves a specific order for a customer.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="customerId">The ID of the customer who owns the order.</param>
        /// <returns>A query result containing the order detail view model if found.</returns>
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
