using data.validata.com.Entities;
using model.validata.com;
using model.validata.com.Enumeration;
namespace business.validata.com.Interfaces
{
    public interface ICustomerBusiness : IAbstractBusiness<Customer>
    {
        Task<ApiResult<Customer>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation); 
    }
}
