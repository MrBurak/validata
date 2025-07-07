using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators.Models;
using data.validata.com.Entities;
using model.validata.com.Enumeration;


namespace business.validata.com.Validators
{
    public class OrderValidation : IOrderValidation
    {
        private readonly IGenericValidation<Customer> genericValidationCustomer;
        private readonly IGenericValidation<Product> genericValidationProduct;
        private readonly IGenericValidation<Order> genericValidationOrder;
        public OrderValidation(IGenericValidation<Customer> genericValidationCustomer, IGenericValidation<Product> genericValidationProduct, IGenericValidation<Order> genericValidationOrder)
        {
            ArgumentNullException.ThrowIfNull(genericValidationCustomer);
            ArgumentNullException.ThrowIfNull(genericValidationProduct);
            ArgumentNullException.ThrowIfNull(genericValidationOrder);
            this.genericValidationCustomer = genericValidationCustomer;
            this.genericValidationProduct = genericValidationProduct;
            this.genericValidationOrder = genericValidationOrder;
        }
        public async Task<OrderValidationResult> InvokeAsync(Order order, BusinessSetOperation businessSetOperation)
        {
            var orderValidationResult = new OrderValidationResult();

            var orderExists = await genericValidationOrder.Exists(order.OrderId, businessSetOperation);
            if (orderExists != null)
            {
                if (orderExists.Entity == null)
                {
                    orderValidationResult.ValidationResult.AddError(orderExists.Code);
                    return orderValidationResult;
                }
                orderValidationResult.ValidationResult.Entity = orderExists.Entity;

            }
            else 
            {
                return orderValidationResult;
            }

                var customerExists = await genericValidationCustomer.Exists(order.CustomerId, BusinessSetOperation.Get);
            if (customerExists != null && customerExists.Entity == null)
            {
                orderValidationResult.ValidationResult.AddError("Customer Not Found");
                return orderValidationResult;
            }
            else 
            {
                if (orderExists.Entity!.CustomerId != customerExists!.Entity!.CustomerId) 
                
                {
                    orderValidationResult.ValidationResult.AddError("Order belongs to another customer");
                    return orderValidationResult;
                }
            }
            if (!order.OrderItems.Any())
            {
                orderValidationResult.ValidationResult.AddError("Order needs to have at least one product");
            }
            else
            {
                List<int> productIds = new List<int>();
                foreach (var item in order.OrderItems)
                {
                    if (productIds.Contains(item.ProductId))
                    {
                        orderValidationResult.ValidationResult.AddError("Same product in multi order item");
                        continue;
                    }
                    if (item.Quantity.Equals(0))
                    {
                        orderValidationResult.ValidationResult.AddError("Item quantity cannot be zero");
                        continue;
                    }
                    var productexists = await genericValidationProduct.Exists(item.ProductId, BusinessSetOperation.Get);
                    if (productexists != null)
                    {
                        if (productexists.Entity == null)
                        {
                            orderValidationResult.ValidationResult.AddError("Product Not Found");
                            continue;
                        }

                        orderValidationResult.Products.Add(productexists.Entity);
                    }
                    productIds.Add(item.ProductId);
                }
            }
            return orderValidationResult;

        }
    }
}
