using model.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using util.validata.com;
using model.validata.com.ValueObjects.OrderItem;
using model.validata.com.ValueObjects.Order;
using business.validata.com.Interfaces.Adaptors;
using model.validata.com.DTO;


namespace business.validata.com.Adaptors
{
    public class OrderAdaptor : IOrderAdaptor
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderItemRepository orderItemRepository;
        public OrderAdaptor(IProductRepository productRepository, IOrderItemRepository orderItemRepository)
        {
            ArgumentNullException.ThrowIfNull(productRepository);
            ArgumentNullException.ThrowIfNull(orderItemRepository);
            this.productRepository = productRepository;
            this.orderItemRepository = orderItemRepository;
        }
        public async Task<Order> Invoke(OrderUpdateModel model, BusinessSetOperation businessSetOperation)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var productIds = model.Items!.Select(x => x.ProductId);
            var products = (await productRepository.GetAllAsync(new PaginationRequest(1, int.MaxValue))).Where(p => productIds.Contains(p.ProductId));
            var orderId = businessSetOperation == BusinessSetOperation.Update ? model.OrderId : 0;
            decimal price = 0;
            decimal calculatedTotalAmount = 0m;
            int calculatedProductCount = 0;
            var newOrderItems = new List<OrderItem>();

            foreach (var itemModel in model.Items!)
            {
                var product = products.FirstOrDefault(p => p.ProductId == itemModel.ProductId);

                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {itemModel.ProductId} not found.");
                }

                ItemProductQuantity itemQuantityVo = new ItemProductQuantity(itemModel.Quantity);
                ItemProductPrice productPriceVo = new ItemProductPrice(product.Price);

                calculatedProductCount += itemQuantityVo.Value;
                calculatedTotalAmount += itemQuantityVo.Value * productPriceVo.Value;

                var orderItem = new OrderItem(
                    productId: itemModel.ProductId,
                    orderId: model.OrderId,
                    quantity: itemQuantityVo,
                    productPrice: productPriceVo
                );
                newOrderItems.Add(orderItem);
            }

            TotalAmount orderTotalAmountVo = new TotalAmount(calculatedTotalAmount);
            ProductQuantity orderProductCountVo = new ProductQuantity(calculatedProductCount);

            var order = new Order(
                orderId: model.OrderId,
                customerId: model.CustomerId,
                orderDate: DateTimeUtil.SystemTime,
                totalAmount: orderTotalAmountVo,
                productQuantity: orderProductCountVo
            );

            foreach (var item in newOrderItems)
            {
                order.AddOrderItem(item);
            }

            return order;


        }
        public IEnumerable<OrderViewModel> Invoke(IEnumerable<OrderDto> orders)
        {
            return orders.Select(order => new OrderViewModel()
            {
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderId = order.OrderId,
                ProductCount = order.ProductQuantity,
            });

        }

        public async Task<OrderDetailViewModel> InvokeAsync(OrderDto order)
        {
            var orderItems= await orderItemRepository.GetAllAsync(order.OrderId);
            var productIds=orderItems.Select(item => item.ProductId);
            var products = (await productRepository.GetAllWithDeletedAsync()).Where(x => productIds.Contains(x.ProductId));

            return new OrderDetailViewModel
            {
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderId = order.OrderId,
                ProductCount = order.ProductQuantity,
                Items = orderItems.Select(item => new OrderItemViewModel 
                {
                    Quantity=item.Quantity,
                    ProductPrice=item.ProductPrice,
                    ProductName=products.FirstOrDefault(products=> products.ProductId== item.ProductId)?.Name.Value
                })
            };
        }
    }
}
