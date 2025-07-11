using business.validata.com;
using business.validata.com.Interfaces;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com;
using Moq;
using test.utils;

namespace business.validata.test
{
    public class CustomerQueryBusinessTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository=new();
        private readonly Mock<ILogger<CustomerQueryBusiness>> _mockLogger=new();
        private readonly Mock<IOrderQueryBusiness> _mockOrderQueryBusiness = new();
        private readonly CustomerQueryBusiness _customerQueryBusiness;
        

        public CustomerQueryBusinessTests()
        {
            
            _customerQueryBusiness = new CustomerQueryBusiness(_mockRepository.Object, _mockLogger.Object, _mockOrderQueryBusiness.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CustomerQueryBusiness(null!, _mockLogger.Object,_mockOrderQueryBusiness.Object));
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
            _mockRepository.Setup(r => r.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(customers);

            var result = await _customerQueryBusiness.ListAsync(PaginationUtil.paginationRequest);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Data);
            var customerViewModels = result.Data!.ToList();
            Assert.Equal(2, customerViewModels.Count);
            Assert.Equal("John", customerViewModels[0].FirstName);
            Assert.Equal("123 Main St", customerViewModels[0].Address);
            Assert.Equal("Jane", customerViewModels[1].FirstName);
            Assert.Equal("456 Oak Ave", customerViewModels[1].Address);
            _mockRepository.Verify(r => r.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);

        }

        [Fact]
        public async Task ListAsync_HandlesRepositoryException()
        {
            var expectedExceptionMessage = "Database error during ListAsync";
            _mockRepository.Setup(r => r.GetAllAsync(PaginationUtil.paginationRequest)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _customerQueryBusiness.ListAsync(PaginationUtil.paginationRequest);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
            _mockRepository.Verify(r => r.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);

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
            Assert.NotNull(result.Data);
            Assert.Equal(customerId, result.Data!.CustomerId);
            Assert.Equal("Test", result.Data.FirstName);
            Assert.Equal("User", result.Data.LastName);
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
            Assert.Null(result.Data);
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
            Assert.Null(result.Data);
            _mockRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);

        }
    }
}
