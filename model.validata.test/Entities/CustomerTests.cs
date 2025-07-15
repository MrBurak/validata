using model.validata.com.ValueObjects.Customer;

namespace model.validata.test.Entities
{


    public class CustomerTests
    {
        private FirstName ValidFirstName() => new FirstName("Burak");
        private LastName ValidLastName() => new LastName("Kepti");
        private EmailAddress ValidEmail() => new EmailAddress("burak.kepti@example.com");
        private StreetAddress ValidAddress() => new StreetAddress("123 Main St");
        private PostalCode ValidPobox() => new PostalCode("A1B 2C3");

        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            var customer = new com.Entities.Customer(
                1,
                ValidFirstName(),
                ValidLastName(),
                ValidEmail(),
                ValidAddress(),
                ValidPobox());

            Assert.Equal(1, customer.CustomerId);
            Assert.Equal("Burak", customer.FirstNameValue);
            Assert.Equal("Kepti", customer.LastNameValue);
            Assert.Equal("burak.kepti@example.com", customer.EmailValue);
            Assert.Equal("123 Main St", customer.AddressValue);
            Assert.Equal("A1B 2C3", customer.PoboxValue);

            Assert.Equal("Burak", customer.FirstName.Value);
            Assert.Equal("Kepti", customer.LastName.Value);
            Assert.Equal("burak.kepti@example.com", customer.Email.Value);
            Assert.Equal("123 Main St", customer.Address.Value);
            Assert.Equal("A1B 2C3", customer.Pobox.Value);
        }

        [Fact]
        public void UpdateFirstName_ShouldUpdateValues()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());
            var newFirstName = new FirstName("Jane");

            customer.UpdateFirstName(newFirstName);

            Assert.Equal("Jane", customer.FirstNameValue);
            Assert.Equal("Jane", customer.FirstName.Value);
        }

        [Fact]
        public void UpdateLastName_ShouldUpdateValues()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());
            var newLastName = new LastName("Smith");

            customer.UpdateLastName(newLastName);

            Assert.Equal("Smith", customer.LastNameValue);
            Assert.Equal("Smith", customer.LastName.Value);
        }

        [Fact]
        public void UpdateEmail_ShouldUpdateValues()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());
            var newEmail = new EmailAddress("jane.smith@example.com");

            customer.UpdateEmail(newEmail);

            Assert.Equal("jane.smith@example.com", customer.EmailValue);
            Assert.Equal("jane.smith@example.com", customer.Email.Value);
        }

        [Fact]
        public void UpdateAddress_ShouldUpdateValues()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());
            var newAddress = new StreetAddress("456 Elm St");

            customer.UpdateAddress(newAddress);

            Assert.Equal("456 Elm St", customer.AddressValue);
            Assert.Equal("456 Elm St", customer.Address.Value);
        }

        [Fact]
        public void UpdatePobox_ShouldUpdateValues()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());
            var newPobox = new PostalCode("Z9Y 8X7");

            customer.UpdatePobox(newPobox);

            Assert.Equal("Z9Y 8X7", customer.PoboxValue);
            Assert.Equal("Z9Y 8X7", customer.Pobox.Value);
        }

        [Fact]
        public void LoadValueObjectsFromBackingFields_ShouldRestoreValueObjects()
        {
            var customer = new com.Entities.Customer(1, ValidFirstName(), ValidLastName(), ValidEmail(), ValidAddress(), ValidPobox());

            customer.UpdateFirstName(new FirstName("a"));
            customer.UpdateLastName(new LastName("a"));
            customer.UpdateEmail(new EmailAddress("a@a.com"));
            customer.UpdateAddress(new StreetAddress("a"));
            customer.UpdatePobox(new PostalCode("a"));

            customer.LoadValueObjectsFromBackingFields();

            Assert.NotNull(customer.FirstName);
            Assert.NotNull(customer.LastName);
            Assert.NotNull(customer.Email);
            Assert.NotNull(customer.Address);
            Assert.NotNull(customer.Pobox);
        }
    }

}
