using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using System.Linq.Expressions;
using util.validata.com;

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
                    x.OperationSourceId = (int)BusinessOperationSource.Api;
                    x.OrderDate = DateTimeUtil.SystemTime;
                    x.ProductCount = order.ProductCount;
                    x.TotalAmount = order.TotalAmount;
                }
            };

                foreach (var item in order.OrderItems)
                {
                    item.DeletedOn = null;
                    item.OperationSourceId = (int)BusinessOperationSource.Api;
                }

                Order? result;

                if (businessSetOperation == BusinessSetOperation.Create)
                {
                    logger.LogInformation("Creating new Order.");
                    result = await unitOfWork.orders.AddAsync(order);
                    await unitOfWork.CommitAsync();
                    logger.LogInformation("Order created with ID: {OrderId}", result?.OrderId);
                }
                else
                {
                    logger.LogInformation("Updating Order with ID: {OrderId}", order.OrderId);
                    var query = genericLambdaExpressions.GetEntityByPrimaryKey(order);
                    await unitOfWork.orders.UpdateAsync(query, properties);
                    await DeleteOrderItemsAsync(x => x.DeletedOn == null && x.OrderId == order.OrderId);
                    await unitOfWork.CommitAsync();
                    result = await unitOfWork.orders.GetEntityAsync(query);
                    logger.LogInformation("Order updated with ID: {OrderId}", order.OrderId);
                }

                order.OrderId = result!.OrderId;

                var orderDetailViewModel = new OrderDetailViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ProductCount = order.ProductCount,
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(x => new OrderItemViewModel
                    {
                        Quantity = x.Quantity,
                        ProductPrice = x.ProductPrice,
                        ProductName = validate.Products.FirstOrDefault()?.Name ?? "",
                    })
                };

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

            List<Action<Order>> properties = new()
        {
            x =>
            {
                x.DeletedOn = DateTimeUtil.SystemTime;
                x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                x.OperationSourceId = (int)BusinessOperationSource.Api;
            }
        };

            try
            {
                logger.LogInformation("Soft deleting Order with ID: {OrderId}", id);
                await unitOfWork.orders.UpdateAsync(genericLambdaExpressions.GetEntityById<Order>(id), properties);
                await DeleteOrderItemsAsync(x => x.DeletedOn == null && x.OrderId == id);
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

            var orders = await DeleteOrdersAsync(expressionOrder);
            if (!orders.Success || orders.Data == null)
            {
                logger.LogWarning("No orders deleted for CustomerId: {CustomerId}", customerId);
                return;
            }

            var orderIds = orders.Data.Select(x => x.OrderId).ToList();
            Expression<Func<OrderItem, bool>> expressionOrderItem = x => orderIds.Contains(x.OrderId) && x.DeletedOn == null;

            await DeleteOrderItemsAsync(expressionOrderItem);
            await unitOfWork.CommitAsync();

            logger.LogInformation("Deleted all orders and order items for CustomerId: {CustomerId}", customerId);
        }

        private async Task<CommandResult<List<Order>>> DeleteOrdersAsync(Expression<Func<Order, bool>> expression)
        {
            CommandResult<List<Order>> commandResult = new();
            List<Action<Order>> properties = new()
        {
            x =>
            {
                x.DeletedOn = DateTimeUtil.SystemTime;
                x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                x.OperationSourceId = (int)BusinessOperationSource.Api;
            }
        };
            try
            {
                logger.LogInformation("Deleting orders matching expression.");
                await unitOfWork.orders.UpdateAsync(expression, properties);
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
            List<Action<OrderItem>> properties = new()
        {
            x =>
            {
                x.DeletedOn = DateTimeUtil.SystemTime;
                x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                x.OperationSourceId = (int)BusinessOperationSource.Api;
            }
        };
            try
            {
                logger.LogInformation("Deleting order items matching expression.");
                await unitOfWork.orderItems.UpdateAsync(expression, properties);
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


    }
}
