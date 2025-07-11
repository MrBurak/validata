using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using data.validata.com.Entities;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Customer;
using model.validata.com.Enumeration;
using Moq;
using test.utils;

namespace api.validata.test
{
    public class CustomerControllerTests
    {
        private readonly Mock<ILogger<CustomerController>> _mockLogger;
        private readonly Mock<ICustomerCommandBusiness> _mockCommandBusiness;
        private readonly Mock<ICustomerQueryBusiness> _mockQueryBusiness;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _mockLogger = new Mock<ILogger<CustomerController>>();
            _mockCommandBusiness = new Mock<ICustomerCommandBusiness>();
            _mockQueryBusiness = new Mock<ICustomerQueryBusiness>();
            _controller = new CustomerController(
                _mockLogger.Object,
                _mockCommandBusiness.Object,
                _mockQueryBusiness.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerController(
                null!, _mockCommandBusiness.Object, _mockQueryBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerController(
                _mockLogger.Object, null!, _mockQueryBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerController(
                _mockLogger.Object, _mockCommandBusiness.Object, null!));
        }

        [Fact]
        public async Task List_ReturnsQueryResultFromQueryBusiness()
        {
            var expectedResult = new QueryResult<IEnumerable<CustomerViewModel>>
            {
                Success = true,
                Data = new List<CustomerViewModel>
            {
                new CustomerViewModel { CustomerId = 1, FirstName = "Test1" },
                new CustomerViewModel { CustomerId = 2, FirstName = "Test2" }
            }
            };

            _mockQueryBusiness.Setup(b => b.ListAsync(PaginationUtil.paginationRequest)).ReturnsAsync(expectedResult);

            var result = await _controller.List(1, 100);

            Assert.Equal(expectedResult, result);
            _mockQueryBusiness.Verify(b => b.ListAsync(PaginationUtil.paginationRequest), Times.Once);
        }

        [Fact]
        public async Task Insert_MapsRequestAndInvokesCommandBusiness()
        {
            var insertModel = new CustomerInsertModel { FirstName = "New Customer", Email = "new@example.com" };
            var expectedCommandResult = new CommandResult<CustomerViewModel>
            {
                Success = true,
                Data = new CustomerViewModel { CustomerId = 1, FirstName = "New Customer" }
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(It.IsAny<Customer>(), BusinessSetOperation.Create))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Insert(insertModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(
                It.Is<Customer>(c => c.FirstName == insertModel.FirstName && c.Email == insertModel.Email),
                BusinessSetOperation.Create), Times.Once);
        }

        [Fact]
        public async Task Update_MapsRequestAndInvokesCommandBusiness()
        {
            var updateModel = new CustomerUpdateModel { CustomerId = 1, FirstName = "Updated Customer"};
            var expectedCommandResult = new CommandResult<CustomerViewModel>
            {
                Success = true,
                Data = new CustomerViewModel { CustomerId = 1, FirstName = "Updated Customer" }
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(It.IsAny<Customer>(), BusinessSetOperation.Update))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Update(updateModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(
                It.Is<Customer>(c => c.CustomerId == updateModel.CustomerId && c.FirstName == updateModel.FirstName),
                BusinessSetOperation.Update), Times.Once);
        }

        [Fact]
        public async Task Delete_InvokesCommandBusinessDelete()
        {
            int customerIdToDelete = 1;
            var expectedCommandResult = new CommandResult<Customer>
            {
                Success = true,
                Data = new Customer { CustomerId = customerIdToDelete, FirstName = "Deleted Customer" }
            };

            _mockCommandBusiness.Setup(b => b.DeleteAsync(customerIdToDelete)).ReturnsAsync(expectedCommandResult);

            var result = await _controller.Delete(customerIdToDelete);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.DeleteAsync(customerIdToDelete), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsQueryResultFromQueryBusiness()
        {
            int customerIdToGet = 1;
            var expectedResult = new QueryResult<CustomerViewModel?>
            {
                Success = true,
                Data = new CustomerViewModel { CustomerId = customerIdToGet, FirstName = "Retrieved Customer" }
            };

            _mockQueryBusiness.Setup(b => b.GetAsync(customerIdToGet)).ReturnsAsync(expectedResult);

            var result = await _controller.Get(customerIdToGet);

            Assert.Equal(expectedResult, result);
            _mockQueryBusiness.Verify(b => b.GetAsync(customerIdToGet), Times.Once);
        }
    }
}
