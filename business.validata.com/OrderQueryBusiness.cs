using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Order;
using model.validata.com;
using util.validata.com;
using model.validata.com.Entities;
using business.validata.com.Interfaces.Validators;
using Microsoft.Extensions.Logging;
using business.validata.com.Interfaces.Adaptors;


namespace business.validata.com
{
    public class OrderQueryBusiness : IOrderQueryBusiness
    {
        private readonly IOrderRepository repository;
        private readonly IOrderItemRepository repositoryItem;
        private readonly IProductRepository repositoryProduct;
        private readonly IGenericValidation<Customer> genericValidationCustomer;
        private readonly ILogger<OrderQueryBusiness> logger;
        private readonly IOrderAdaptor orderAdaptor;

        public OrderQueryBusiness
        (
            IOrderRepository repository,
            IOrderItemRepository repositoryItem,
            IProductRepository repositoryProduct,
            IGenericValidation<Customer> genericValidationCustomer,
            ILogger<OrderQueryBusiness> logger,
            IOrderAdaptor orderAdaptor
            )
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(repositoryItem);
            ArgumentNullException.ThrowIfNull(repositoryProduct);
            ArgumentNullException.ThrowIfNull(genericValidationCustomer);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(orderAdaptor);
            this.repository = repository;
            this.repositoryItem = repositoryItem;
            this.repositoryProduct = repositoryProduct;
            this.genericValidationCustomer = genericValidationCustomer;
            this.logger = logger;
            this.orderAdaptor = orderAdaptor;
        }
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> ListAsync(int customerId, PaginationRequest paginationRequest)
        {
            logger.LogInformation("Listing orders for CustomerId={CustomerId}, Page={PageNumber}, Size={PageSize}", customerId, paginationRequest.pageNumber, paginationRequest.pageSize);

            var queryResult = new QueryResult<IEnumerable<OrderViewModel>>();
            var customerExists = await genericValidationCustomer.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get);

            if (customerExists != null && customerExists.Entity == null)
            {
                logger.LogWarning("Customer not found with ID={CustomerId}", customerId);
                queryResult.Exception = "Customer Not Found";
                return queryResult;
            }

            try
            {
                var orders = orderAdaptor.Invoke(await repository.GetAllAsync(customerId, paginationRequest)).ToList();

                if (orders.Any())
                {
                    queryResult.Data = orders;
                    queryResult.Success = true;
                    logger.LogInformation("Found {Count} orders for CustomerId={CustomerId}", orders.Count, customerId);
                }
                else
                {
                    logger.LogInformation("No orders found for CustomerId={CustomerId}", customerId);
                    queryResult.Exception = "No record found";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while listing orders for CustomerId={CustomerId}", customerId);
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;
        }

        public async Task<QueryResult<OrderDetailViewModel?>> GetAsync(int orderId, int customerId)
        {
            logger.LogInformation("Getting order details for OrderId={OrderId}, CustomerId={CustomerId}", orderId, customerId);

            var queryResult = new QueryResult<OrderDetailViewModel?>();
            try
            {
                var order = await repository.GetByIdAsync(orderId, customerId);
                if (order == null)
                {
                    logger.LogWarning("Order not found with OrderId={OrderId}, CustomerId={CustomerId}", orderId, customerId);
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    return queryResult;
                }

                var orderModel = await orderAdaptor.InvokeAsync(order);

                var orderItems = (await repositoryItem.GetAllAsync(customerId))
                    .Where(x => x.OrderId == orderId)
                    .ToList();

                var productIds = orderItems.Select(o => o.ProductId).Distinct().ToList();
                var products = (await repositoryProduct.GetAllWithDeletedAsync())
                    .Where(x => productIds.Contains(x.ProductId))
                    .ToList();

                orderModel.Items = orderItems.Select(x => new OrderItemViewModel
                {
                    ProductPrice = x.ProductPrice,
                    Quantity = x.Quantity,
                    ProductName = products.FirstOrDefault(p => p.ProductId == x.ProductId)?.Name ?? string.Empty,
                });

                queryResult.Data = orderModel;
                queryResult.Success = true;

                logger.LogInformation("Retrieved order details for OrderId={OrderId}, CustomerId={CustomerId}", orderId, customerId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting order details for OrderId={OrderId}, CustomerId={CustomerId}", orderId, customerId);
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;
        }
    }
}
