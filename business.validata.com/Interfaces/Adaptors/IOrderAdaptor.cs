using model.validata.com.DTO;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Order;


namespace business.validata.com.Interfaces.Adaptors
{
    public interface IOrderAdaptor
    {
        Task<Order> Invoke(OrderUpdateModel model, BusinessSetOperation businessSetOperation);

        IEnumerable<OrderViewModel> Invoke(IEnumerable<OrderDto> orders);

        Task<OrderDetailViewModel> InvokeAsync(OrderDto order);
        
    }
}
