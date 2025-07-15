using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Customer;
using Moq;
using System.Linq.Expressions;


namespace business.validata.test.Validators
{


    public class CustomerValidationTests
    {
        private readonly Mock<IGenericValidation<Customer>> genericValidationMock = new();
        private readonly Mock<ICommandRepository<Customer>> repositoryMock = new();

        private readonly CustomerValidation sut;

        public CustomerValidationTests()
        {
            sut = new CustomerValidation(genericValidationMock.Object, repositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenGenericValidationIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerValidation(null!, repositoryMock.Object));
            Assert.Contains("genericValidation", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerValidation(genericValidationMock.Object, null!));
            Assert.Contains("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_Succeeds_WhenDependenciesAreProvided()
        {
            var validation = new CustomerValidation(genericValidationMock.Object, repositoryMock.Object);
            Assert.NotNull(validation);
        }

        [Fact]
        public async Task Should_ReturnError_IfCustomerEntityDoesNotExist()
        {
            var customer = CreateCustomer();

            genericValidationMock.Setup(g => g.Exists(customer, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Update);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("No record found");
        }

        [Fact]
        public async Task Should_ReturnFieldValidationErrors()
        {
            var customer = CreateCustomer();

            genericValidationMock.Setup(g => g.Exists(customer, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });

            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.FirstName), true, null))
                .Returns("First name is required");

            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.LastName), true, null))
                .Returns("Last name is required");

            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.Pobox), true, null))
                .Returns("PO Box is required");

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Update);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("First name is required");
            result.Errors.Should().Contain("Last name is required");
            result.Errors.Should().Contain("PO Box is required");
        }

        [Fact]
        public async Task Should_ReturnError_IfEmailIsNotUnique()
        {
            var customer = CreateCustomer("existing@example.com");

            genericValidationMock.Setup(g => g.Exists(customer, BusinessSetOperation.Create))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });

            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.FirstName), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.LastName), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.Pobox), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.Email), true, It.IsAny<string>()))
                .Returns((string?)null);

            repositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>
                {
                CreateCustomer("existing@example.com", 99) 
                });

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Create);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("Customer email have to be unique");
        }

        [Fact]
        public async Task Should_PassValidation_WhenValid()
        {
            var customer = CreateCustomer("unique@example.com");

            genericValidationMock.Setup(g => g.Exists(customer, BusinessSetOperation.Create))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });

            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.FirstName), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.LastName), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.Pobox), true, null)).Returns((string?)null);
            genericValidationMock.Setup(g => g.ValidateStringField(customer, nameof(Customer.Email), true, It.IsAny<string>()))
                .Returns((string?)null);

            repositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>()); 

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Create);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        private Customer CreateCustomer(string email = "test@example.com", int id = 1)
        {
            return new Customer(id, new FirstName("John"), new LastName("Doe"), new EmailAddress(email), new StreetAddress("Street 1"), new PostalCode("123"));
        }
    }

}
