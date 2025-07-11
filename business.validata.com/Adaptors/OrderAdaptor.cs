using business.validata.com.Interfaces;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;

using util.validata.com;

namespace business.validata.com.Adaptors
{
    public class OrderAdaptor : IOrderAdaptor
    {
        private readonly IProductRepository productRepository;
        public OrderAdaptor(IProductRepository productRepository)
        {
            ArgumentNullException.ThrowIfNull(productRepository);
            this.productRepository = productRepository;
        }
        public async Task<Order> Invoke(OrderUpdateModel model, BusinessSetOperation businessSetOperation) 
        {
            if (model == null) 
            {
                throw new ArgumentNullException(nameof(model));
            }
            var productIds =model.Items!.Select(x => x.ProductId);
            var products = (await productRepository.GetAllAsync(new PaginationRequest(1, int.MaxValue))).Where(p => productIds.Contains(p.ProductId));
            var orderId = businessSetOperation == BusinessSetOperation.Update ? model.OrderId : 0;
            float price = 0;
            var order = new Order
            {
                OrderId = orderId,
                CustomerId = model.CustomerId,
                OrderDate = DateTimeUtil.SystemTime,
                ProductCount = model.Items!.Sum(x => x.Quantity),
                OrderItems = model.Items!.Select(orderItem => new OrderItem
                {
                    OrderId = orderId,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    ProductPrice = products.FirstOrDefault(product => product.ProductId == orderItem.ProductId)?.Price ?? price,
                    
                }
                ).ToList()
            };
            order.TotalAmount=order.OrderItems.Sum(x => x.Quantity * x.ProductPrice);
            return order;
        }
    }
}
