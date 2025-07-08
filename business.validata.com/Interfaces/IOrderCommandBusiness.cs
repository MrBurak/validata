using data.validata.com.Entities;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Order;
namespace business.validata.com.Interfaces
{
    public interface IOrderCommandBusiness : IAbstractCommandBusiness<Order>
    {
        Task<CommandResult<OrderDetailViewModel>> InvokeAsync(OrderUpdateModel orderUpdateModel, BusinessSetOperation businessSetOperation);
        Task DeleteAllAsync(int customerId);
    }
}
