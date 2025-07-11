using data.validata.com.Entities;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
namespace business.validata.com.Interfaces
{
    public interface ICustomerCommandBusiness
    {
        Task<CommandResult<CustomerViewModel>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation);
        Task<CommandResult<Customer>> DeleteAsync(int id);
    }
}
