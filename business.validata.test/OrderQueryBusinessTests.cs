using business.validata.com;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com.Validators;
using Moq;
using test.utils;

namespace business.validata.test
{
    public class OrderQueryBusinessTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IOrderItemRepository> _mockOrderItemRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IGenericValidation<Customer>> _mockGenericValidationCustomer;
        private readonly Mock<ILogger<OrderQueryBusiness>> _mockLogger = new();
        private readonly OrderQueryBusiness _orderQueryBusiness;

        public OrderQueryBusinessTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockGenericValidationCustomer = new Mock<IGenericValidation<Customer>>();

            _orderQueryBusiness = new OrderQueryBusiness(
                _mockOrderRepository.Object,
                _mockOrderItemRepository.Object,
                _mockProductRepository.Object,
                _mockGenericValidationCustomer.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderQueryBusiness(
                null!, _mockOrderItemRepository.Object, _mockProductRepository.Object, _mockGenericValidationCustomer.Object, _mockLogger.Object));
           
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOrderItemRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderQueryBusiness(
                _mockOrderRepository.Object, null!, _mockProductRepository.Object, _mockGenericValidationCustomer.Object, _mockLogger.Object));
            
            Assert.Equal("repositoryItem", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfProductRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderQueryBusiness(
                _mockOrderRepository.Object, _mockOrderItemRepository.Object, null!, _mockGenericValidationCustomer.Object, _mockLogger.Object));
           
            Assert.Equal("repositoryProduct", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationCustomerIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderQueryBusiness(
                _mockOrderRepository.Object, _mockOrderItemRepository.Object, _mockProductRepository.Object, null!, _mockLogger.Object));
           
            Assert.Equal("genericValidationCustomer", ex.ParamName);
        }


        [Fact]
        public async Task ListAsync_ReturnsCustomerNotFound_WhenCustomerDoesNotExist()
        {
            int customerId = 100;
            _mockGenericValidationCustomer.Setup(gv => gv.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await _orderQueryBusiness.ListAsync(customerId, PaginationUtil.paginationRequest);

            Assert.False(result.Success);
            Assert.Equal("Customer Not Found", result.Exception);
            Assert.Null(result.Data);
            _mockGenericValidationCustomer.Verify(gv => gv.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get), Times.Once);
            _mockOrderRepository.Verify(r => r.GetAllAsync(It.IsAny<int>(), PaginationUtil.paginationRequest), Times.Never);
        }

        [Fact]
        public async Task ListAsync_ReturnsOrdersSuccessfully_WhenOrdersExist()
        {
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId };
            var orders = new List<Order>
        {
            new Order { OrderId = 1, CustomerId = customerId, OrderDate = new DateTime(2023, 1, 1), TotalAmount = 50.00f },
            new Order { OrderId = 2, CustomerId = customerId, OrderDate = new DateTime(2023, 1, 15), TotalAmount = 75.00f }
        };

            _mockGenericValidationCustomer.Setup(gv => gv.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });
            _mockOrderRepository.Setup(r => r.GetAllAsync(customerId, PaginationUtil.paginationRequest)).ReturnsAsync(orders);

            var result = await _orderQueryBusiness.ListAsync(customerId, PaginationUtil.paginationRequest);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Data);
            var orderViewModels = result.Data!.ToList();
            Assert.Equal(2, orderViewModels.Count);
            Assert.Equal(1, orderViewModels[0].OrderId);
            _mockOrderRepository.Verify(r => r.GetAllAsync(customerId, PaginationUtil.paginationRequest), Times.Once);
        }

        [Fact]
        public async Task ListAsync_ReturnsEmptyList_WhenNoOrdersExistForCustomer()
        {
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId };
            var orders = new List<Order>();

            _mockGenericValidationCustomer.Setup(gv => gv.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });
            _mockOrderRepository.Setup(r => r.GetAllAsync(customerId, PaginationUtil.paginationRequest)).ReturnsAsync(orders);

            var result = await _orderQueryBusiness.ListAsync(customerId, PaginationUtil.paginationRequest);

            Assert.False(result.Success);
            Assert.NotNull(result.Exception);
            Assert.Equal("No record found", result.Exception);
            
        }

        [Fact]
        public async Task ListAsync_HandlesRepositoryException()
        {
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId };
            var expectedExceptionMessage = "Database error during ListAsync";

            _mockGenericValidationCustomer.Setup(gv => gv.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = customer });
            _mockOrderRepository.Setup(r => r.GetAllAsync(customerId, PaginationUtil.paginationRequest)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _orderQueryBusiness.ListAsync(customerId, PaginationUtil.paginationRequest);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
            _mockOrderRepository.Verify(r => r.GetAllAsync(customerId, PaginationUtil.paginationRequest), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNoRecordFound_WhenOrderDoesNotExist()
        {
            int orderId = 1, customerId = 1;
            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId, customerId)).ReturnsAsync((Order?)null);

            var result = await _orderQueryBusiness.GetAsync(orderId, customerId);

            Assert.False(result.Success);
            Assert.Equal("No record found", result.Exception);
            Assert.Null(result.Data);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId, customerId), Times.Once);
            _mockOrderItemRepository.Verify(r => r.GetAllAsync(It.IsAny<int>()), Times.Never);
            _mockProductRepository.Verify(r => r.GetAllWithDeletedAsync(), Times.Never);
        }

        

        [Fact]
        public async Task GetAsync_HandlesRepositoryException()
        {
            int orderId = 1, customerId = 1;
            var expectedExceptionMessage = "Database error during GetAsync";
            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId, customerId)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _orderQueryBusiness.GetAsync(orderId, customerId);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId, customerId), Times.Once);
            _mockOrderItemRepository.Verify(r => r.GetAllAsync(It.IsAny<int>()), Times.Never);
            _mockProductRepository.Verify(r => r.GetAllWithDeletedAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAsync_OrderItemsAreEmpty_ReturnsOrderDetailWithEmptyItems()
        {
            int orderId = 1, customerId = 1;
            var order = new Order { OrderId = orderId, CustomerId = customerId, OrderDate = DateTime.Now, TotalAmount = 0.00f};
            var emptyOrderItems = new List<OrderItem>();
            var products = new List<Product>(); 

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId, customerId)).ReturnsAsync(order);
            _mockOrderItemRepository.Setup(r => r.GetAllAsync(customerId)).ReturnsAsync(emptyOrderItems);
            _mockProductRepository.Setup(r => r.GetAllWithDeletedAsync()).ReturnsAsync(products);

            var result = await _orderQueryBusiness.GetAsync(orderId, customerId);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Data);
            Assert.Equal(orderId, result.Data!.OrderId);
            Assert.Empty(result.Data.Items);
            _mockOrderRepository.Verify(r => r.GetByIdAsync(orderId, customerId), Times.Once);
            _mockOrderItemRepository.Verify(r => r.GetAllAsync(customerId), Times.Once);
            _mockProductRepository.Verify(r => r.GetAllWithDeletedAsync(), Times.Once); 
        }
}







}
