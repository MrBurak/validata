using business.validata.com.Interfaces.Adaptors;
using model.validata.com.Customer;
using model.validata.com.DTO;
using model.validata.com.Entities;
using model.validata.com.ValueObjects.Customer;

namespace business.validata.com.Adaptors
{
    public class CustomerAdaptor: ICustomerAdaptor
    {
        public CustomerDto Invoke(Customer customer) 
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Email=customer.Email.Value,
                Address = customer.Address.Value,
                FirstName=customer.FirstName.Value,
                LastName=customer.LastName.Value,   
                Pobox=customer.Pobox.Value,
            };
        }

        public Customer Invoke(CustomerInsertModel customer)
        {
            return new Customer(0, new FirstName(customer.FirstName!), new LastName(customer.LastName!), new EmailAddress(customer.Email!), new StreetAddress(customer.Address!), new PostalCode(customer.Pobox!));
        }

        public Customer Invoke(CustomerUpdateModel customer)
        {
            return new Customer(customer.CustomerId, new FirstName(customer.FirstName!), new LastName(customer.LastName!), new EmailAddress("fake@email.com"), new StreetAddress(customer.Address!), new PostalCode(customer.Pobox!));
        }
    }
}
