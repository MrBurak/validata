using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Adaptors;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Order;
using Moq;

namespace business.validata.test
{
  

    public class CustomerQueryBusinessTests
    {
        private readonly Mock<ICustomerRepository> repositoryMock = new();
        private readonly Mock<ILogger<CustomerQueryBusiness>> loggerMock = new();
        private readonly Mock<IOrderQueryBusiness> orderQueryMock = new();
        private readonly Mock<ICustomerAdaptor> adaptorMock = new();

        private readonly CustomerQueryBusiness sut;

        public CustomerQueryBusinessTests()
        {
            sut = new CustomerQueryBusiness(
                repositoryMock.Object,
                loggerMock.Object,
                orderQueryMock.Object,
                adaptorMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerQueryBusiness(
                    null!,
                    loggerMock.Object,
                    orderQueryMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerQueryBusiness(
                    repositoryMock.Object,
                    null!,
                    orderQueryMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfOrderQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerQueryBusiness(
                    repositoryMock.Object,
                    loggerMock.Object,
                    null!,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerQueryBusiness(
                    repositoryMock.Object,
                    loggerMock.Object,
                    orderQueryMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WithAllValidDependencies()
        {
            var sut = new CustomerQueryBusiness(
                repositoryMock.Object,
                loggerMock.Object,
                orderQueryMock.Object,
                adaptorMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnCustomers_WhenRepositorySucceeds()
        {
            var pagination = new PaginationRequest(1, 10);
            var customers = new List<CustomerDto> { new() { CustomerId = 1, FirstName = "Alice" } };

            repositoryMock.Setup(r => r.GetAllAsync(pagination)).ReturnsAsync(customers);

            var result = await sut.ListAsync(pagination);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(customers);
        }

        [Fact]
        public async Task ListAsync_ShouldHandleRepositoryException()
        {
            var pagination = new PaginationRequest(1, 10);

            repositoryMock.Setup(r => r.GetAllAsync(pagination)).ThrowsAsync(new Exception("Database error"));

            var result = await sut.ListAsync(pagination);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Database error");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCustomer_WhenFound()
        {
            var customer = new CustomerDto { CustomerId = 1, FirstName = "John", LastName="Doe" };

            repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

            var result = await sut.GetAsync(1);

            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(customer);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnError_WhenCustomerNotFound()
        {
            repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((CustomerDto?)null);

            var result = await sut.GetAsync(999);

            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Exception.Should().Be("No record found");
        }

        [Fact]
        public async Task GetAsync_ShouldHandleException()
        {
            repositoryMock.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("Boom"));

            var result = await sut.GetAsync(1);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Boom");
        }

        [Fact]
        public async Task ListOrderAsync_ShouldDelegateToOrderQueryBusiness()
        {
            var customerId = 1;
            var pagination = new PaginationRequest(1, 10);
            var expected = new QueryResult<IEnumerable<OrderViewModel>> { Success = true };

            orderQueryMock.Setup(o => o.ListAsync(customerId, pagination)).ReturnsAsync(expected);

            var result = await sut.ListOrderAsync(customerId, pagination);

            result.Should().BeSameAs(expected);
        }
    }

}
