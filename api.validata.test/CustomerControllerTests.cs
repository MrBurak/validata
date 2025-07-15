using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using model.validata.com.Entities;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
using Moq;
using business.validata.com.Interfaces.Adaptors;
using model.validata.com.DTO;
using model.validata.com.Order;

namespace api.validata.test
{


    public class CustomerControllerTests
    {
        private readonly Mock<ILogger<CustomerController>> _loggerMock= new();
        private readonly Mock<ICustomerCommandBusiness> _commandMock = new();
        private readonly Mock<ICustomerQueryBusiness> _queryMock = new();
        private readonly Mock<ICustomerAdaptor> _adaptorMock = new();
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            

            _controller = new CustomerController(
                _loggerMock.Object,
                _commandMock.Object,
                _queryMock.Object,
                _adaptorMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerController(
                    null!,
                    _commandMock.Object,
                    _queryMock.Object,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerController(
                    _loggerMock.Object,
                    null!,
                    _queryMock.Object,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerController(
                    _loggerMock.Object,
                    _commandMock.Object,
                    null!,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerController(
                    _loggerMock.Object,
                    _commandMock.Object,
                    _queryMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
        {
            var controller = new CustomerController(
                _loggerMock.Object,
                _commandMock.Object,
                _queryMock.Object,
                _adaptorMock.Object);

            Assert.NotNull(controller);
        }

        [Fact]
        public async Task List_ReturnsCustomers_WhenQueryIsSuccessful()
        {
            
            var expected = new QueryResult<IEnumerable<CustomerDto>> { Success = true, Data = new List<CustomerDto> { new() } };
            _queryMock.Setup(q => q.ListAsync(It.IsAny<PaginationRequest>())).ReturnsAsync(expected);

            
            var result = await _controller.List(1, 10);

            
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Insert_ReturnsSuccess_WhenCommandSucceeds()
        {
            
            var insertModel = new CustomerInsertModel { Email = "test@example.com" };
            var customer = new Customer();
            var expected = new CommandResult<CustomerDto> { Success = true };

            _adaptorMock.Setup(a => a.Invoke(insertModel)).Returns(customer);
            _commandMock.Setup(c => c.InvokeAsync(customer, BusinessSetOperation.Create)).ReturnsAsync(expected);

            
            var result = await _controller.Insert(insertModel);

            
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Update_ReturnsSuccess_WhenUpdateIsValid()
        {
            var updateModel = new CustomerUpdateModel { CustomerId = 1 };
            var customer = new Customer();
            var expected = new CommandResult<CustomerDto> { Success = true };

            _adaptorMock.Setup(a => a.Invoke(updateModel)).Returns(customer);
            _commandMock.Setup(c => c.InvokeAsync(customer, BusinessSetOperation.Update)).ReturnsAsync(expected);

            var result = await _controller.Update(updateModel);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess_WhenCustomerDeleted()
        {
            var expected = new CommandResult<Customer> { Success = true };
            _commandMock.Setup(c => c.DeleteAsync(1)).ReturnsAsync(expected);

            var result = await _controller.Delete(1);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Get_ReturnsCustomer_WhenFound()
        {
            var expected = new QueryResult<CustomerDto?> { Success = true, Data = new CustomerDto() };
            _queryMock.Setup(q => q.GetAsync(1)).ReturnsAsync(expected);

            var result = await _controller.Get(1);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ListOrders_ReturnsOrders_WhenFound()
        {
            var expected = new QueryResult<IEnumerable<OrderViewModel>> { Success = true, Data = new List<OrderViewModel> { new() } };
            _queryMock.Setup(q => q.ListOrderAsync(1, It.IsAny<PaginationRequest>())).ReturnsAsync(expected);

            var result = await _controller.ListOrders(1, 1, 10);

            Assert.True(result.Success);
            Assert.NotEmpty(result.Data!);
        }
    }

}
