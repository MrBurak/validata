using model.validata.com.Entities;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.DTO;
namespace business.validata.com.Interfaces
{
    public interface ICustomerCommandBusiness
    {
        Task<CommandResult<CustomerDto>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation);
        Task<CommandResult<Customer>> DeleteAsync(int id);
    }
}
