using data.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Order;


namespace business.validata.com.Interfaces
{
    public interface IOrderAdaptor
    {
        Task<Order> Invoke(OrderUpdateModel model, BusinessSetOperation businessSetOperation);
    }
}
