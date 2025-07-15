using business.validata.com;
using business.validata.com.Interfaces.Adaptors;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Entities;
using model.validata.com.Order;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Product;
using Moq;

namespace business.validata.test
{


    public class OrderQueryBusinessTests
    {
        private readonly Mock<IOrderRepository> orderRepoMock = new();
        private readonly Mock<IOrderItemRepository> orderItemRepoMock = new();
        private readonly Mock<IProductRepository> productRepoMock = new();
        private readonly Mock<IGenericValidation<Customer>> validationMock = new();
        private readonly Mock<ILogger<OrderQueryBusiness>> loggerMock = new();
        private readonly Mock<IOrderAdaptor> adaptorMock = new();

        private readonly OrderQueryBusiness sut;

        public OrderQueryBusinessTests()
        {
            sut = new OrderQueryBusiness(
                orderRepoMock.Object,
                orderItemRepoMock.Object,
                productRepoMock.Object,
                validationMock.Object,
                loggerMock.Object,
                adaptorMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfOrderRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    null!,
                    orderItemRepoMock.Object,
                    productRepoMock.Object,
                    validationMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfOrderItemRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    orderRepoMock.Object,
                    null!,
                    productRepoMock.Object,
                    validationMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfProductRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    orderRepoMock.Object,
                    orderItemRepoMock.Object,
                    null!,
                    validationMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfGenericValidationCustomerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    orderRepoMock.Object,
                    orderItemRepoMock.Object,
                    productRepoMock.Object,
                    null!,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    orderRepoMock.Object,
                    orderItemRepoMock.Object,
                    productRepoMock.Object,
                    validationMock.Object,
                    null!,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfOrderAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderQueryBusiness(
                    orderRepoMock.Object,
                    orderItemRepoMock.Object,
                    productRepoMock.Object,
                    validationMock.Object,
                    loggerMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreValid()
        {
            var sut = new OrderQueryBusiness(
                orderRepoMock.Object,
                orderItemRepoMock.Object,
                productRepoMock.Object,
                validationMock.Object,
                loggerMock.Object,
                adaptorMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnOrders_WhenCustomerExists()
        {
            var customerId = 1;
            var pagination = new PaginationRequest(1, 10);
            var orderDtos = new List<OrderDto> { new OrderDto { OrderId = 1, CustomerId = 1 } };
            var orderViewModels = new List<OrderViewModel> { new OrderViewModel { OrderId = 1 } };

            validationMock.Setup(v => v.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer() });

            orderRepoMock.Setup(r => r.GetAllAsync(customerId, pagination)).ReturnsAsync(orderDtos);
            adaptorMock.Setup(a => a.Invoke(orderDtos)).Returns(orderViewModels);

            var result = await sut.ListAsync(customerId, pagination);

            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnException_WhenCustomerNotFound()
        {
            var customerId = 1;
            var pagination = new PaginationRequest(1, 10);

            validationMock.Setup(v => v.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = null });

            var result = await sut.ListAsync(customerId, pagination);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Customer Not Found");
        }

        [Fact]
        public async Task ListAsync_ShouldReturnException_WhenRepositoryThrows()
        {
            var customerId = 1;
            var pagination = new PaginationRequest(1, 10);

            validationMock.Setup(v => v.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer() });

            orderRepoMock.Setup(r => r.GetAllAsync(customerId, pagination)).ThrowsAsync(new Exception("DB Error"));

            var result = await sut.ListAsync(customerId, pagination);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("DB Error");
        }

        [Fact]
        public async Task ListAsync_ShouldReturnMessage_WhenNoOrders()
        {
            var customerId = 1;
            var pagination = new PaginationRequest(1, 10);

            validationMock.Setup(v => v.Exists(customerId, model.validata.com.Enumeration.BusinessSetOperation.Get))
                .ReturnsAsync(new ExistsResult<Customer> { Entity = new Customer() });

            orderRepoMock.Setup(r => r.GetAllAsync(customerId, pagination)).ReturnsAsync(new List<OrderDto>());
            adaptorMock.Setup(a => a.Invoke(It.IsAny<IEnumerable<OrderDto>>())).Returns(new List<OrderViewModel>());

            var result = await sut.ListAsync(customerId, pagination);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("No record found");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnOrderDetails_WhenOrderExists()
        {
            var orderId = 100;
            var customerId = 1;
            var order = new OrderDto { OrderId = orderId, CustomerId = customerId };
            var detailModel = new OrderDetailViewModel();

            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 10, OrderId = orderId, Quantity = 1, ProductPrice = 9.99m }
            };

                var products = new List<Product>
            {
                new Product(10, new ProductName("Item A"), new ProductPrice(9.99m))
            };

            orderRepoMock.Setup(r => r.GetByIdAsync(orderId, customerId)).ReturnsAsync(order);
            adaptorMock.Setup(a => a.InvokeAsync(order)).ReturnsAsync(detailModel);
            orderItemRepoMock.Setup(r => r.GetAllAsync(customerId)).ReturnsAsync(orderItems);
            productRepoMock.Setup(p => p.GetAllWithDeletedAsync()).ReturnsAsync(products);

            var result = await sut.GetAsync(orderId, customerId);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Items.Should().HaveCount(1);
            result.Data.Items.First().ProductName.Should().Be("Item A");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnError_WhenOrderNotFound()
        {
            orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((OrderDto?)null);

            var result = await sut.GetAsync(1, 1);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("No record found");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnError_WhenRepositoryThrows()
        {
            orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Fetch failed"));

            var result = await sut.GetAsync(1, 1);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Fetch failed");
        }
    }




}
