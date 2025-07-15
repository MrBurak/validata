using model.validata.com.DTO;


namespace model.validata.test.DTO
{
    public class CustomerDtoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAllProperties()
        {
            var customer = new CustomerDto
            {
                CustomerId = 1,
                FirstName = "Burak",
                LastName = "Kepti",
                Email = "burak@example.com",
                Address = "123 Main St",
                Pobox = "12345"
            };

            
            Assert.Equal(1, customer.CustomerId);
            Assert.Equal("Burak", customer.FirstName);
            Assert.Equal("Kepti", customer.LastName);
            Assert.Equal("burak@example.com", customer.Email);
            Assert.Equal("123 Main St", customer.Address);
            Assert.Equal("12345", customer.Pobox);
        }

        [Fact]
        public void DefaultValues_ShouldNotBeNull()
        {
            
            var customer = new CustomerDto();

            
            Assert.NotNull(customer.FirstName);
            Assert.NotNull(customer.LastName);
            Assert.NotNull(customer.Email);
            Assert.NotNull(customer.Address);
            Assert.NotNull(customer.Pobox);
        }

        [Fact]
        public void PropertySetters_ShouldWorkCorrectly()
        {
            
            var customer = new CustomerDto();

            
            customer.FirstName = "Test";
            customer.LastName = "User";
            customer.Email = "test@user.com";
            customer.Address = "456 Test Ave";
            customer.Pobox = "99999";

            
            Assert.Equal("Test", customer.FirstName);
            Assert.Equal("User", customer.LastName);
            Assert.Equal("test@user.com", customer.Email);
            Assert.Equal("456 Test Ave", customer.Address);
            Assert.Equal("99999", customer.Pobox);
        }
    }
}
