using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using model.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;
using business.validata.com.Interfaces.Adaptors;
using model.validata.com.ValueObjects.Customer;
using model.validata.com.DTO;
using FluentAssertions;
using business.validata.com.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace business.validata.test
{
    

    public class CustomerCommandBusinessTests
    {
        private readonly Mock<ICustomerValidation> validationMock = new();
        private readonly Mock<ICommandRepository<Customer>> customerRepoMock = new();
        private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
        private readonly Mock<IGenericLambdaExpressions> lambdaMock = new();
        private readonly Mock<IGenericValidation<Customer>> genericValidationMock = new();
        private readonly Mock<IOrderCommandBusiness> orderCommandMock = new();
        private readonly Mock<ICustomerAdaptor> adaptorMock = new();
        private readonly Mock<ILogger<CustomerCommandBusiness>> loggerMock = new();

        private readonly CustomerCommandBusiness sut;

        public CustomerCommandBusinessTests()
        {
            unitOfWorkMock.SetupGet(u => u.customers).Returns(customerRepoMock.Object);

            sut = new CustomerCommandBusiness(
                validationMock.Object,
                customerRepoMock.Object,
                genericValidationMock.Object,
                lambdaMock.Object,
                orderCommandMock.Object,
                unitOfWorkMock.Object,
                loggerMock.Object,
                adaptorMock.Object
            );
        }
        [Fact]
        public void Constructor_Throws_IfValidationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerCommandBusiness(null!, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                            orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfRepositoryNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, null!, genericValidationMock.Object, lambdaMock.Object,
                                            orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfGenericValidationNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, null!, lambdaMock.Object,
                                            orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfLambdaNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, null!,
                                            orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfOrderCommandBusinessNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                            null!, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfUnitOfWorkNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                            orderCommandMock.Object, null!, loggerMock.Object, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfLoggerNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                            orderCommandMock.Object, unitOfWorkMock.Object, null!, adaptorMock.Object));
        }

        [Fact]
        public void Constructor_Throws_IfAdaptorNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                            orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, null!));
        }

        [Fact]
        public void Constructor_Succeeds_WithValidDependencies()
        {
            var sut = new CustomerCommandBusiness(validationMock.Object, customerRepoMock.Object, genericValidationMock.Object, lambdaMock.Object,
                                                  orderCommandMock.Object, unitOfWorkMock.Object, loggerMock.Object, adaptorMock.Object);

            Assert.NotNull(sut);
        }


        [Fact]
        public async Task InvokeAsync_ShouldCreateCustomer_WhenValid()
        {
            var customer = new Customer(0, new FirstName("John"), new LastName("Doe"), new EmailAddress("a@b.com"), new StreetAddress("a"), new PostalCode("a"));
            var dto = new CustomerDto { CustomerId = 1 };
            var vr = new ValidationResult<Customer>();
            validationMock.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Create))
                .ReturnsAsync(new ValidationResult<Customer>());

            customerRepoMock.Setup(r => r.AddAsync(customer)).ReturnsAsync(customer);
            adaptorMock.Setup(a => a.Invoke(customer)).Returns(dto);

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Create);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(dto);
        }

        [Fact]
        public async Task InvokeAsync_ShouldUpdateCustomer_WhenValid()
        {
            var customer = new Customer(42, new FirstName("John"), new LastName("Doe"), new EmailAddress("a@b.com"), new StreetAddress("a"), new PostalCode("a"));
            var dto = new CustomerDto { CustomerId = 42 };
            var id = 10;
            Expression<Func<Customer, bool>> query = x => x.DeletedOn == null && x.CustomerId == 42;

            validationMock.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Customer>());

            lambdaMock.Setup(l => l.GetEntityByPrimaryKey(customer)).Returns(query);
            customerRepoMock.Setup(r => r.GetEntityAsync(query)).ReturnsAsync(customer);
            adaptorMock.Setup(a => a.Invoke(customer)).Returns(dto);

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Update);

            result.Success.Should().BeTrue();
            result.Data.Should().Be(dto);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnValidationErrors_WhenInvalid()
        {
            var customer = new Customer();
            var validationResult = new ValidationResult<Customer>();
            validationResult.Errors.Add("Name is required");

            validationMock.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Create))
                .ReturnsAsync(validationResult);

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Create);

            result.Success.Should().BeFalse();
            result.Validations.Should().Contain("Name is required");
        }

        [Fact]
        public async Task InvokeAsync_ShouldHandleException()
        {
            var customer = new Customer(0, new FirstName("John"), new LastName("Doe"), new EmailAddress("a@b.com"), new StreetAddress("a"), new PostalCode("a"));


            validationMock.Setup(v => v.InvokeAsync(customer, BusinessSetOperation.Create))
                .ReturnsAsync(new ValidationResult<Customer>());

            customerRepoMock.Setup(r => r.AddAsync(customer)).ThrowsAsync(new Exception("DB failure"));

            var result = await sut.InvokeAsync(customer, BusinessSetOperation.Create);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("DB failure");
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCustomer_WhenValid()
        {
            var id = 10;
            Expression<Func<Customer, bool>> query = x => x.DeletedOn == null && x.CustomerId == id;

            genericValidationMock.Setup(v => v.Exists(id, BusinessSetOperation.Delete))
                .ReturnsAsync((ExistsResult<Customer>?)null);

            lambdaMock.Setup(l => l.GetEntityById<Customer>(id)).Returns(query);

            var result = await sut.DeleteAsync(id);

            result.Success.Should().BeTrue();

            customerRepoMock.Verify(r => r.DeleteAsync(query), Times.Once);
            orderCommandMock.Verify(o => o.DeleteAllAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnValidationError_WhenInvalid()
        {
            var id = 10;

            genericValidationMock.Setup(v => v.Exists(id, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await sut.DeleteAsync(id);

            result.Success.Should().BeFalse();
            result.Validations.Should().Contain("No record found");
        }

        [Fact]
        public async Task DeleteAsync_ShouldHandleException()
        {
            var id = 10;
            Expression<Func<Customer, bool>> query = x=> x.DeletedOn==null && x.CustomerId==id;

            genericValidationMock.Setup(v => v.Exists(id, BusinessSetOperation.Delete)).ReturnsAsync((ExistsResult<Customer>?)null);
            lambdaMock.Setup(l => l.GetEntityById<Customer>(id)).Returns(query);
            customerRepoMock.Setup(r => r.DeleteAsync(query)).ThrowsAsync(new Exception("Delete failed"));

            var result = await sut.DeleteAsync(id);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Delete failed");
        }
    }

}

