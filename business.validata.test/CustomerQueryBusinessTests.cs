using business.validata.com;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace business.validata.test
{
    public class CustomerQueryBusinessTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository;
        private readonly Mock<ILogger<CustomerQueryBusiness>> _mockLogger;
        private readonly CustomerQueryBusiness _customerQueryBusiness;

        public CustomerQueryBusinessTests()
        {
            _mockRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<CustomerQueryBusiness>>();
            _customerQueryBusiness = new CustomerQueryBusiness(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerQueryBusiness(null!, _mockLogger.Object));
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public async Task ListAsync_ReturnsAllCustomersSuccessfully()
        {
            var customers = new List<Customer>
        {
            new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Address = "123 Main St", Pobox = "1001" },
            new Customer { CustomerId = 2, FirstName = "Jane", LastName = "Smith", Address = "456 Oak Ave", Pobox = "2002" }
        };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            var result = await _customerQueryBusiness.ListAsync();

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Result);
            var customerViewModels = result.Result!.ToList();
            Assert.Equal(2, customerViewModels.Count);
            Assert.Equal("John", customerViewModels[0].FirstName);
            Assert.Equal("123 Main St", customerViewModels[0].Address);
            Assert.Equal("Jane", customerViewModels[1].FirstName);
            Assert.Equal("456 Oak Ave", customerViewModels[1].Address);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);

        }

        [Fact]
        public async Task ListAsync_HandlesRepositoryException()
        {
            var expectedExceptionMessage = "Database error during ListAsync";
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _customerQueryBusiness.ListAsync();

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);

        }

        [Fact]
        public async Task GetAsync_ReturnsCustomerSuccessfully()
        {
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId, FirstName = "Test", LastName = "User", Address = "789 Pine", Pobox = "3003" };
            _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(customer);

            var result = await _customerQueryBusiness.GetAsync(customerId);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Result);
            Assert.Equal(customerId, result.Result!.CustomerId);
            Assert.Equal("Test", result.Result.FirstName);
            Assert.Equal("User", result.Result.LastName);
            _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
 
        }

        [Fact]
        public async Task GetAsync_ReturnsNoRecordFound_WhenCustomerDoesNotExist()
        {
            int customerId = 99;
            _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);

            var result = await _customerQueryBusiness.GetAsync(customerId);

            Assert.False(result.Success);
            Assert.Equal("No record found", result.Exception);
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
           
        }

        [Fact]
        public async Task GetAsync_HandlesRepositoryException()
        {
            int customerId = 1;
            var expectedExceptionMessage = "Network error during GetAsync";
            _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _customerQueryBusiness.GetAsync(customerId);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);

        }
    }
}
