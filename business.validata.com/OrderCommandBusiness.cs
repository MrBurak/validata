using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using System.Linq.Expressions;
using util.validata.com;

namespace business.validata.com
{
    public class OrderCommandBusiness : AbstractCommandBusiness<Order>, IOrderCommandBusiness
    {
        
        private readonly IOrderValidation validation;
        private readonly IOrderItemCommandBusiness orderItemCommandBusiness;
        private readonly IOrderAdaptor orderAdaptor;
        public OrderCommandBusiness(
            IOrderValidation validation,
            ICommandRepository<Order> repository,
            IGenericValidation<Order> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions,
            IOrderItemCommandBusiness orderItemCommandBusiness,
            IOrderAdaptor orderAdaptor) :
            base(genericValidation, repository, genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(validation);
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            ArgumentNullException.ThrowIfNull(orderAdaptor);
            ArgumentNullException.ThrowIfNull(orderItemCommandBusiness);
            this.validation = validation;
            this.orderItemCommandBusiness = orderItemCommandBusiness;
            this.orderAdaptor = orderAdaptor;
        }

        public async Task<CommandResult<OrderDetailViewModel>> InvokeAsync(OrderUpdateModel orderUpdateModel, BusinessSetOperation businessSetOperation)
        {
            CommandResult<OrderDetailViewModel> commandResult = new CommandResult<OrderDetailViewModel>();
            var order= await orderAdaptor.Invoke(orderUpdateModel, businessSetOperation);

            var validate = await validation.InvokeAsync(order, businessSetOperation);
            if (!validate.ValidationResult.IsValid) 
            {
                commandResult.Validations=validate.ValidationResult.Errors;
                return commandResult;
            }
            try
            {
                List<Action<Order>> properties = new()
                {
                    x=>
                    {
                        x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                        x.OperationSourceId = (int) BusinessOperationSource.Api;
                        x.OrderDate=DateTimeUtil.SystemTime;
                        x.ProductCount=order.ProductCount;
                        x.TotalAmount=order.TotalAmount;

                    }
                };
               
                var result = await InvokeAsync(validate.ValidationResult.Entity!, order, businessSetOperation, properties);
                order = result!;
                var orderitems = await orderItemCommandBusiness.AddAsync(order);
                var orderDetailViewModel = new OrderDetailViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ProductCount = order.ProductCount,
                    TotalAmount = order.TotalAmount,
                    Items = orderitems.Select(x => new OrderItemViewModel
                    {
                        Quantity = x.Quantity,
                        ProductPrice = x.ProductPrice,
                        ProductName = validate.Products.FirstOrDefault()?.Name??"",
                    })
                };
                commandResult.Result = orderDetailViewModel;
                commandResult.Success = true;
            }
            catch (Exception ex)
            {
                commandResult.Exception = ex.Message;
                commandResult.Success = false;
            }
            return commandResult;
        }

        public override async Task<CommandResult<Order>> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            await orderItemCommandBusiness.DeleteAllAsync(id);
            return result;
        }

        public async Task DeleteAllAsync(int customerId)
        {
            Expression<Func<Order, bool>> expression = x => x.CustomerId == customerId && x.DeletedOn == null;
            await DeleteAllAsync(expression);
            await orderItemCommandBusiness.DeleteAllForCustomerAsync(customerId);
        }

        
    }
}
