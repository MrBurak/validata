using model.validata.com.Customer;
using model.validata.com.DTO;
using model.validata.com.Entities;


namespace business.validata.com.Interfaces.Adaptors
{
    public interface ICustomerAdaptor
    {
        CustomerDto Invoke(Customer customer);
        Customer Invoke(CustomerInsertModel customer);
        Customer Invoke(CustomerUpdateModel customer);
    }
}
