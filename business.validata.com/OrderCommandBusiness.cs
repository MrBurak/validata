using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using model.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using System.Linq.Expressions;
using util.validata.com;
using business.validata.com.Interfaces.Adaptors;

namespace business.validata.com
{
    public class OrderCommandBusiness : IOrderCommandBusiness
    {
        
        private readonly IOrderValidation validation;
        private readonly IUnitOfWork unitOfWork;
        private readonly IOrderAdaptor orderAdaptor;
        private readonly IGenericLambdaExpressions genericLambdaExpressions;
        private readonly IGenericValidation<Order> genericValidation;
        private readonly ILogger<OrderCommandBusiness> logger;
        public OrderCommandBusiness(
            IOrderValidation validation,
            ICommandRepository<Order> repository,
            IGenericValidation<Order> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions,
            IUnitOfWork unitOfWork,
            IOrderAdaptor orderAdaptor,
            ILogger<OrderCommandBusiness> logger) 
        {
            ArgumentNullException.ThrowIfNull(validation);
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(orderAdaptor);
            ArgumentNullException.ThrowIfNull(unitOfWork);
            ArgumentNullException.ThrowIfNull(logger);

            this.validation = validation;
            this.unitOfWork = unitOfWork;
            this.orderAdaptor = orderAdaptor;
            this.genericLambdaExpressions = genericLambdaExpressions;
            this.genericValidation = genericValidation;
            this.logger = logger;
        }

        public async Task<CommandResult<OrderDetailViewModel>> InvokeAsync(OrderUpdateModel orderUpdateModel, BusinessSetOperation businessSetOperation)
        {
            logger.LogInformation("Starting InvokeAsync for Order with operation: {Operation}", businessSetOperation);
            CommandResult<OrderDetailViewModel> commandResult = new CommandResult<OrderDetailViewModel>();

            var order = await orderAdaptor.Invoke(orderUpdateModel, businessSetOperation);

            var validate = await validation.InvokeAsync(order, businessSetOperation);
            if (!validate.ValidationResult.IsValid)
            {
                logger.LogWarning("Validation failed for Order invoke. Errors: {@Errors}", validate.ValidationResult.Errors);
                commandResult.Validations = validate.ValidationResult.Errors;
                return commandResult;
            }

            try
            {
                List<Action<Order>> properties = new()
                {
                    x =>
                    {
                            x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                            x.UpdateProductCount(order.ProductQuantity); 
                            x.UpdateTotalAmount(order.TotalAmount);   
                        }
                    };

                foreach (var item in order.OrderItems)
                {
                    item.DeletedOn = null;
                    item.OperationSourceId = (int)BusinessOperationSource.Api;
                }

                if (businessSetOperation == BusinessSetOperation.Create)
                {
                    logger.LogInformation("Creating new Order.");
                    order = await unitOfWork.orders.AddAsync(order);
                    await unitOfWork.CommitAsync();
                    logger.LogInformation("Order created with ID: {OrderId}", order!.OrderId);
                }
                else
                {
                    logger.LogInformation("Updating Order with ID: {OrderId}", order.OrderId);
                    var query = genericLambdaExpressions.GetEntityByPrimaryKey(order);
                    
                    await unitOfWork.orders.UpdateAsync(query, properties);
                    
                    await DeleteOrderItemsAsync(x => x.DeletedOn == null && x.OrderId == order.OrderId);
                    foreach (var item in order.OrderItems)
                    {
                        await unitOfWork.orderItems.AddAsync(item);
                    }
                    await unitOfWork.CommitAsync();
                    order = await unitOfWork.orders.GetEntityAsync(query);
                    logger.LogInformation("Order updated with ID: {OrderId}", order!.OrderId);
                }

                var orderDetailViewModel = new OrderDetailViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ProductCount = order.ProductQuantityValue,
                    TotalAmount = order.TotalAmountValue,
                    Items = order.OrderItems.Where(x=> x.QuantityValue>0).Select(x => new OrderItemViewModel
                    {
                        Quantity = x.QuantityValue,
                        ProductPrice = x.ProductPriceValue,
                        ProductName = validate.Products.FirstOrDefault()?.Name ?? "",
                    })
                };
                orderDetailViewModel.ProductCount = orderDetailViewModel.Items.Sum(x => x.Quantity);
                orderDetailViewModel.TotalAmount = orderDetailViewModel.Items.Sum(x => x.Quantity * x.ProductPrice);

                commandResult.Data = orderDetailViewModel;
                commandResult.Success = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during InvokeAsync for Order.");
                commandResult.Exception = ex.Message;
                commandResult.Success = false;
            }
            return commandResult;
        }

        public async Task<CommandResult<Order>> DeleteAsync(int id)
        {
            logger.LogInformation("Starting DeleteAsync for Order with ID: {OrderId}", id);

            CommandResult<Order> apiResult = new CommandResult<Order>();

            var exist = await genericValidation.Exists(id, BusinessSetOperation.Delete);
            if (exist != null && exist.Code != null)
            {
                logger.LogWarning("Delete validation failed for Order ID: {OrderId}. Validation code: {Code}", id, exist.Code);
                apiResult.Validations.Add(exist.Code);
                return apiResult;
            }

            

            try
            {
                logger.LogInformation("Soft deleting Order with ID: {OrderId}", id);
                await DeleteOrderItemsAsync(x => x.DeletedOn == null && x.OrderId == id);
                await unitOfWork.orders.DeleteAsync(genericLambdaExpressions.GetEntityById<Order>(id));
                
                await unitOfWork.CommitAsync();
                apiResult.Success = true;
                logger.LogInformation("Order with ID: {OrderId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during DeleteAsync for Order with ID: {OrderId}", id);
                apiResult.Exception = ex.Message;
                apiResult.Success = false;
            }

            return apiResult;
        }

        public async Task DeleteAllAsync(int customerId)
        {
            logger.LogInformation("Starting DeleteAllAsync for CustomerId: {CustomerId}", customerId);

            Expression<Func<Order, bool>> expressionOrder = x => x.CustomerId == customerId && x.DeletedOn == null;
            var orderIds = (await unitOfWork.orders.GetListAsync(x=> x.CustomerId==customerId)).Select(x => x.OrderId).ToList();

            Expression<Func<OrderItem, bool>> expressionOrderItem = x => orderIds.Contains(x.OrderId) && x.DeletedOn == null;

            await DeleteOrderItemsAsync(expressionOrderItem);

            var orders = await DeleteOrdersAsync(expressionOrder);
            if (!orders.Success || orders.Data == null)
            {
                logger.LogWarning("No orders deleted for CustomerId: {CustomerId}", customerId);
                return;
            }

            
          
            await unitOfWork.CommitAsync();

            logger.LogInformation("Deleted all orders and order items for CustomerId: {CustomerId}", customerId);
        }

        private async Task<CommandResult<List<Order>>> DeleteOrdersAsync(Expression<Func<Order, bool>> expression)
        {
            CommandResult<List<Order>> commandResult = new();
            
            try
            {
                logger.LogInformation("Deleting orders matching expression.");
                await unitOfWork.orders.DeleteAsync(expression);
                commandResult.Success = true;
                logger.LogInformation("Orders deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting orders.");
                commandResult.Exception = ex.Message;
                commandResult.Success = false;
            }
            return commandResult;
        }

        private async Task<CommandResult<List<OrderItem>>> DeleteOrderItemsAsync(Expression<Func<OrderItem, bool>> expression)
        {
            CommandResult<List<OrderItem>> commandResult = new();
            
            try
            {
                logger.LogInformation("Deleting order items matching expression.");
                await unitOfWork.orderItems.DeleteAsync(expression);
                commandResult.Success = true;
                logger.LogInformation("Order items deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting order items.");
                commandResult.Exception = ex.Message;
                commandResult.Success = false;
            }
            return commandResult;
        }

#if DEBUG
        public async Task<CommandResult<List<Order>>> InvokeDeleteOrdersForTest(Expression<Func<Order, bool>> expr) => await DeleteOrdersAsync(expr);
        public async Task<CommandResult<List<OrderItem>>> InvokeDeleteOrderItemsForTest(Expression<Func<OrderItem, bool>> expr) => await DeleteOrderItemsAsync(expr);
#endif



    }
}
