using business.validata.com.Adaptors;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using model.validata.com.ValueObjects.Product;
using Moq;


namespace business.validata.test.Adaptors
{



    public class OrderAdaptorTests
    {
        private readonly Mock<IProductRepository> productRepoMock = new();
        private readonly Mock<IOrderItemRepository> orderItemRepoMock = new();

        private readonly OrderAdaptor adaptor;

        public OrderAdaptorTests()
        {
            adaptor = new OrderAdaptor(productRepoMock.Object, orderItemRepoMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenProductRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderAdaptor(null!, orderItemRepoMock.Object));
            Assert.Contains("productRepository", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenOrderItemRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new OrderAdaptor(productRepoMock.Object, null!));
            Assert.Contains("orderItemRepository", ex.ParamName);
        }

        [Fact]
        public void Constructor_Succeeds_WhenDependenciesAreProvided()
        {
            var adaptor = new OrderAdaptor(productRepoMock.Object, orderItemRepoMock.Object);
            Assert.NotNull(adaptor);
        }

        [Fact]
        public async Task Invoke_WithValidModelAndProducts_ShouldReturnOrder()
        {
            
            var model = new OrderUpdateModel
            {
                OrderId = 1,
                CustomerId = 123,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 2 },
                new OrderItemInsertModel { ProductId = 2, Quantity = 3 }
            }
            };

            var products = new List<Product>
            {
                new Product (1, new ProductName("a"), new ProductPrice(10)),
                new Product (2, new ProductName("b"), new ProductPrice(5))
            };

            productRepoMock.Setup(p => p.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(products);

            
            var result = await adaptor.Invoke(model, BusinessSetOperation.Create);

            
            result.Should().NotBeNull();
            result.TotalAmount.Value.Should().Be(2 * 10 + 3 * 5);
            result.ProductQuantity.Value.Should().Be(5);
            result.OrderItems.Count.Should().Be(2);
        }

        [Fact]
        public async Task Invoke_WithNullModel_ShouldThrowArgumentNullException()
        {
            
            Func<Task> act = () => adaptor.Invoke(null!, BusinessSetOperation.Create);

            
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Invoke_WhenProductNotFound_ShouldThrowInvalidOperationException()
        {
            
            var model = new OrderUpdateModel
            {
                OrderId = 1,
                CustomerId = 123,
                Items = new List<OrderItemInsertModel> { new OrderItemInsertModel { ProductId = 99, Quantity = 1 } }
            };

            productRepoMock.Setup(p => p.GetAllAsync(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(new List<Product>());

            
            Func<Task> act = () => adaptor.Invoke(model, BusinessSetOperation.Create);

            
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Product with ID 99*");
        }

        [Fact]
        public void Invoke_WithOrderDtos_ShouldMapToViewModels()
        {
            
            var dtos = new List<OrderDto>
            {
                new OrderDto { OrderId = 1, TotalAmount = 100, OrderDate = DateTime.Today, ProductQuantity = 2 },
                new OrderDto { OrderId = 2, TotalAmount = 50, OrderDate = DateTime.Today, ProductQuantity = 1 }
            };

            
            var result = adaptor.Invoke(dtos);

            
            result.Should().HaveCount(2);
            result.First().OrderId.Should().Be(1);
        }

        [Fact]
        public async Task InvokeAsync_WithValidOrder_ShouldReturnDetailViewModel()
        {
            
            var order = new OrderDto
            {
                OrderId = 10,
                TotalAmount = 150,
                OrderDate = DateTime.Today,
                ProductQuantity = 3
            };

            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto{ OrderId=10, OrderItemId=1, ProductId=1, ProductPrice=10, Quantity=1 },
                new OrderItemDto{ OrderId=10, OrderItemId=2, ProductId=2, ProductPrice=20, Quantity=2 },
            };

            var products = new List<Product>
            {
                new Product (1, new ProductName("Product A"), new ProductPrice(10)),
                new Product (2, new ProductName("Product B"), new ProductPrice(10))
            };

            orderItemRepoMock.Setup(r => r.GetAllAsync(order.OrderId)).Returns(Task.FromResult(orderItems.AsEnumerable()));
            productRepoMock.Setup(p => p.GetAllWithDeletedAsync()).ReturnsAsync(products);

            
            var result = await adaptor.InvokeAsync(order);

            
            result.OrderId.Should().Be(10);
            result.Items.Should().HaveCount(2);
            result.Items.First().ProductName.Should().Be("Product A");
        }
    }

}
