using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;


namespace business.validata.test.Validators
{
    public class CustomerValidationTests
    {
        private readonly Mock<IGenericValidation<Customer>> _mockGenericValidation;
        private readonly Mock<ICommandRepository<Customer>> _mockRepository;
        private readonly CustomerValidation _customerValidation;

        public CustomerValidationTests()
        {
            _mockGenericValidation = new Mock<IGenericValidation<Customer>>();
            _mockRepository = new Mock<ICommandRepository<Customer>>();
            _customerValidation = new CustomerValidation(_mockGenericValidation.Object, _mockRepository.Object);

            
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new  ExistsResult<Customer> { Entity = new Customer() }); 

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                    It.IsAny<Customer>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<string>()))
                .ReturnsAsync("");

            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>()); 
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationIsNull()
        {
            IGenericValidation<Customer> nullGenericValidation = null!;
            var mockRepository = new Mock<ICommandRepository<Customer>>();

            Assert.Throws<ArgumentNullException>(() => new CustomerValidation(nullGenericValidation, mockRepository.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            var mockGenericValidation = new Mock<IGenericValidation<Customer>>();
            Assert.Throws<ArgumentNullException>(() => new CustomerValidation(mockGenericValidation.Object, null!));
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_SetsCustomerIdToZero()
        {
            var customer = new Customer { CustomerId = 123 }; 

            await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Create);

            Assert.Equal(0, customer.CustomerId);
        }

        [Fact]
        public async Task InvokeAsync_UpdateOperation_DoesNotSetCustomerIdToZero()
        {
            
            var customer = new Customer { CustomerId = 123 }; 

            
            await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Update);

            
            Assert.Equal(123, customer.CustomerId); 
        }

        [Fact]
        public async Task InvokeAsync_ExistsReturnsNullEntity_AddsErrorAndReturnsImmediately()
        {
            
            var customer = new Customer();
            var expectedErrorCode = "No record found";
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            
            var result = await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Update); 

            
            Assert.False(result.IsValid);
            Assert.Contains(expectedErrorCode, result.Errors);
            Assert.Null(result.Entity); 
            _mockGenericValidation.Verify(m => m.ValidateStringField(
                It.IsAny<Customer>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ExistsReturnsEntity_SetsEntityInValidationResult()
        {
            
            var customer = new Customer();
            var existingCustomer = new Customer { CustomerId = 1, FirstName = "Existing" };
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = existingCustomer });

            
            var result = await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Update);

            
            Assert.Same(existingCustomer, result.Entity);
        }

        [Theory]
        [InlineData("FirstName")]
        [InlineData("LastName")]
        [InlineData("Pobox")]
        public async Task InvokeAsync_AlwaysValidatesRequiredStringFields(string fieldName)
        {
            
            var customer = new Customer();

            
            await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Create);

            
            _mockGenericValidation.Verify(m => m.ValidateStringField(
                customer, fieldName, true, false, null, null), Times.Once);
        }

       

        [Fact]
        public async Task InvokeAsync_UpdateOperation_DoesNotValidateEmailUniqueness()
        {
            
            var customer = new Customer();

            
            await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Update);

            
            _mockRepository.Verify(m => m.GetListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>()), Times.Never);
            _mockGenericValidation.Verify(m => m.ValidateStringField(
                customer,
                nameof(Customer.Email),
                It.IsAny<bool>(),
                true, 
                It.IsAny<List<int>>(),
                It.IsAny<string>()), Times.Never); 
        }

        [Fact]
        public async Task InvokeAsync_AllValidationsPass_ReturnsValidResult()
        {
            
            var customer = new Customer { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Pobox = "12345" };

            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer }); 

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                    It.IsAny<Customer>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(""); 

            
            var result = await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Create);

            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Equal(customer, result.Entity);
        }

        [Fact]
        public async Task InvokeAsync_ValidationErrorsOccur_ReturnsInvalidResultWithErrors()
        {
            
            var customer = new Customer { FirstName = "", LastName = "Doe", Email = "invalid-email", Pobox = "12345" };
            var firstNameError = "FirstName is required.";
            var emailError = "Email format is invalid or not unique.";

            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Customer>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                customer, nameof(Customer.FirstName), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(firstNameError);

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                customer, nameof(Customer.Email), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(emailError);

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                customer, nameof(Customer.LastName), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(""); 

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                customer, nameof(Customer.Pobox), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(""); 


            
            var result = await _customerValidation.InvokeAsync(customer, BusinessSetOperation.Create);

            
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count); 
            Assert.Contains(firstNameError, result.Errors);
            Assert.Contains(emailError, result.Errors);
        }
    }
}
