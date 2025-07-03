using data.validata.com.Entities;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
namespace business.validata.com.Interfaces
{
    public interface ICustomerCommandBusiness : IAbstractCommandBusiness<Customer>
    {
        Task<CommandResult<CustomerViewModel>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation); 
    }
}
