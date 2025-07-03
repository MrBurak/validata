using business.validata.com.Interfaces;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Order;
using model.validata.com;
using util.validata.com;
using data.validata.com.Entities;
using business.validata.com.Interfaces.Validators;
using Microsoft.EntityFrameworkCore;


namespace business.validata.com
{
    public class OrderQueryBusiness : IOrderQueryBusiness
    {
        private readonly IOrderRepository repository;
        private readonly IOrderItemRepository repositoryItem;
        private readonly IProductRepository repositoryProduct;
        private readonly IGenericValidation<Customer> genericValidationCustomer;
        public OrderQueryBusiness
        (
            IOrderRepository repository, 
            IOrderItemRepository repositoryItem,
            IProductRepository repositoryProduct,
            IGenericValidation<Customer> genericValidationCustomer
            )
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(repositoryItem);
            ArgumentNullException.ThrowIfNull(repositoryProduct);
            ArgumentNullException.ThrowIfNull (genericValidationCustomer);
            this.repository = repository;
            this.repositoryItem = repositoryItem;
            this.repositoryProduct = repositoryProduct;
            this.genericValidationCustomer = genericValidationCustomer;
        }
        public async Task<QueryResult<IEnumerable<OrderViewModel>>> ListAsync(int customerId)
        {
            var queryResult = new QueryResult<IEnumerable<OrderViewModel>>();
            var customerExists=await genericValidationCustomer.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get);

            if (customerExists != null && customerExists.Entity == null) 
            {
                queryResult.Exception= customerExists.Code;
                return queryResult;
            }
            try
            {
                var orders= ObjectUtil.ConvertObj<IEnumerable<OrderViewModel>, IEnumerable<Order>>(await repository.GetAllAsync(customerId)).ToList();

                var orderItems=(await repositoryItem.GetAllAsync(customerId)).Where(x=> orders.Select(x=> x.OrderId).Contains(x.OrderId));

                orders.ForEach(r => r.TotalPrice = orderItems.Where(i => i.OrderId == r.OrderId).Sum(i => i.ProductPrice * i.Quantity));

                if (orders.Any())
                {
                    queryResult.Result = orders;
                    queryResult.Success = true;
                }
                
            }
            catch (Exception ex) 
            {
                queryResult.Success = false;
                queryResult.Exception=ex.Message;
            }
           return queryResult;
        }

        public async Task<QueryResult<OrderDetailViewModel?>> GetAsync(int orderId, int customerId)
        {
            var queryResult = new QueryResult<OrderDetailViewModel?>();
            try 
            {
                var order = (await repository.GetByIdAsync(orderId, customerId));
                if (order == null) 
                {
                    queryResult.Exception = "No record found";
                    queryResult.Success = false;
                    return queryResult;
                }
                var orderModel= ObjectUtil.ConvertObj<OrderDetailViewModel, Order>(order);
                
                var orderItems = (await repositoryItem.GetAllAsync(customerId)).Where(x => x.OrderId==orderId);
                var products = (await repositoryProduct.GetAllAsync()).Where(x => orderItems.Select(o=> o.ProductId).Contains(x.ProductId));
                orderModel.TotalPrice = orderItems.Sum(x => x.Quantity * x.ProductPrice);
                orderModel.Items = orderItems.Select(x=> new OrderItemViewModel 
                { 
                    ProductPrice = x.ProductPrice,  
                    Quantity = x.Quantity,  
                    ProductName=products.FirstOrDefault(p=> p.ProductId==x.ProductId)?.Name,
                });



                queryResult.Result = ObjectUtil.ConvertObj<OrderDetailViewModel, Order>(order);
                queryResult.Success = false;
            }
            catch (Exception ex)
            {
                queryResult.Success = false;
                queryResult.Exception = ex.Message;
            }
            return queryResult;

        }
    }
}
