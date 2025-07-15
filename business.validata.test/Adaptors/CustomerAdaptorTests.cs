using business.validata.com.Adaptors;
using model.validata.com.Customer;
using model.validata.com.Entities;
using model.validata.com.ValueObjects.Customer;


namespace business.validata.test.Adaptors
{

    public class CustomerAdaptorTests
    {
        private readonly CustomerAdaptor adaptor = new();

        [Fact]
        public void Invoke_Should_Map_Customer_To_CustomerDto()
        {
            var customer = new Customer(
                1,
                new FirstName("John"),
                new LastName("Doe"),
                new EmailAddress("john.doe@example.com"),
                new StreetAddress("123 Street"),
                new PostalCode("12345"));

            var dto = adaptor.Invoke(customer);

            Assert.Equal(1, dto.CustomerId);
            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("john.doe@example.com", dto.Email);
            Assert.Equal("123 Street", dto.Address);
            Assert.Equal("12345", dto.Pobox);
        }

        [Fact]
        public void Invoke_Should_Map_InsertModel_To_Customer()
        {
            var model = new CustomerInsertModel
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                Address = "456 Lane",
                Pobox = "67890"
            };

            var customer = adaptor.Invoke(model);

            Assert.Equal("Alice", customer.FirstName.Value);
            Assert.Equal("Smith", customer.LastName.Value);
            Assert.Equal("alice@example.com", customer.Email.Value);
            Assert.Equal("456 Lane", customer.Address.Value);
            Assert.Equal("67890", customer.Pobox.Value);
        }

        [Fact]
        public void Invoke_Should_Map_UpdateModel_To_Customer()
        {
            var model = new CustomerUpdateModel
            {
                CustomerId = 10,
                FirstName = "Bob",
                LastName = "Brown",
                Address = "789 Avenue",
                Pobox = "54321"
            };

            var customer = adaptor.Invoke(model);

            Assert.Equal(10, customer.CustomerId);
            Assert.Equal("Bob", customer.FirstName.Value);
            Assert.Equal("Brown", customer.LastName.Value);
            Assert.Equal("789 Avenue", customer.Address.Value);
            Assert.Equal("54321", customer.Pobox.Value);
            Assert.Equal("fake@email.com", customer.Email.Value);
        }
    }

}
