using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using System.Linq.Expressions;


namespace business.validata.com
{
    public class OrderItemCommandBusiness : AbstractCommandBusiness<OrderItem>, IOrderItemCommandBusiness
    {
        private readonly ICommandRepository<OrderItem> repository;
        private readonly ICommandRepository<Order> repositoryOrder;
        public OrderItemCommandBusiness(
            ICommandRepository<OrderItem> repository,
            ICommandRepository<Order> repositoryOrder,
            IGenericValidation<OrderItem> genericValidation,
            IGenericLambdaExpressions genericLambdaExpressions): base(genericValidation, repository, genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(repositoryOrder);
            this.repository = repository;
            this.repositoryOrder = repositoryOrder;
        } 
        public async Task<List<OrderItem>> AddAsync(Order order)
        {
            Expression<Func<OrderItem, bool>> expression= x=> x.OrderId == order.OrderId && x.DeletedOn ==null;
            await DeleteAllAsync(expression);

            List<OrderItem> orderItems= new List<OrderItem>();
            foreach (var item in order.OrderItems) 
            {
               orderItems.Add( await repository.AddAsync(item));

            }
            return orderItems;
        }

        public async Task DeleteAllAsync(int orderId)
        {
            Expression<Func<OrderItem, bool>> expression = x => x.OrderId == orderId && x.DeletedOn == null;
            await DeleteAllAsync(expression);
        }

        public async Task DeleteAllForCustomerAsync(int customerId)
        {
            Expression<Func<Order, bool>> expression = x => x.CustomerId == customerId && x.DeletedOn == null;
            var orderIds = (await repositoryOrder.GetListAsync(expression)).Select(x=> x.OrderId).ToList();
            Expression<Func<OrderItem, bool>> expressionItem = x => orderIds.Contains(x.OrderId) && x.DeletedOn == null;
            await DeleteAllAsync(expressionItem);
        }
    }
}
