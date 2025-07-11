using business.validata.com.Adaptors;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Order;
using Moq;
using test.utils;


namespace business.validata.test.Adaptors
{

    public class OrderAdaptorTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly OrderAdaptor _orderAdaptor;


        public OrderAdaptorTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _orderAdaptor = new OrderAdaptor(_mockProductRepository.Object);

        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenProductRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAdaptor(null!));
        }

        [Fact]
        public async Task Invoke_CreatesNewOrder_WhenBusinessSetOperationIsCreate()
        {
            var products = new List<Product>
                {
                    new Product { ProductId = 1, Name = "Laptop", Price = 1000.0f },
                    new Product { ProductId = 2, Name = "Mouse", Price = 25.0f }
                };
            _mockProductRepository.Setup(repo => repo.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(products);

            var model = new OrderUpdateModel
            {
                CustomerId = 101,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 2 },
                new OrderItemInsertModel { ProductId = 2, Quantity = 1 }
            }
            };

            
            var order = await _orderAdaptor.Invoke(model, BusinessSetOperation.Create);

            
            Assert.NotNull(order);
            Assert.Equal(0, order.OrderId); 
            Assert.Equal(model.CustomerId, order.CustomerId);
            Assert.Equal(3, order.ProductCount); 
            Assert.Equal(2, order.OrderItems.Count);

            var orderItem1 = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 1);
            Assert.NotNull(orderItem1);
            Assert.Equal(1, orderItem1.ProductId);
            Assert.Equal(2, orderItem1.Quantity);
            Assert.Equal(1000.0f, orderItem1.ProductPrice);

            var orderItem2 = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 2);
            Assert.NotNull(orderItem2);
            Assert.Equal(2, orderItem2.ProductId);
            Assert.Equal(1, orderItem2.Quantity);
            Assert.Equal(25.0f, orderItem2.ProductPrice);

            Assert.Equal((2 * 1000.0f) + (1 * 25.0f), order.TotalAmount); 
            _mockProductRepository.Verify(repo => repo.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);
        }

        [Fact]
        public async Task Invoke_UpdatesExistingOrder_WhenBusinessSetOperationIsUpdate()
        {
            
            var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Price = 1000.0f },
            new Product { ProductId = 3, Name = "Keyboard", Price = 75.0f }
        };
            _mockProductRepository.Setup(repo => repo.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(products);

            var model = new OrderUpdateModel
            {
                OrderId = 123, 
                CustomerId = 101,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 1 },
                new OrderItemInsertModel { ProductId = 3, Quantity = 3 }
            }
            };

            
            var order = await _orderAdaptor.Invoke(model, BusinessSetOperation.Update);

            
            Assert.NotNull(order);
            Assert.Equal(model.OrderId, order.OrderId); 
            Assert.Equal(model.CustomerId, order.CustomerId);
            Assert.Equal(4, order.ProductCount); 
            Assert.Equal(2, order.OrderItems.Count);

            var orderItem1 = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 1);
            Assert.NotNull(orderItem1);
            Assert.Equal(1, orderItem1.ProductId);
            Assert.Equal(1, orderItem1.Quantity);
            Assert.Equal(1000.0f, orderItem1.ProductPrice);

            var orderItem2 = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 3);
            Assert.NotNull(orderItem2);
            Assert.Equal(3, orderItem2.ProductId);
            Assert.Equal(3, orderItem2.Quantity);
            Assert.Equal(75.0f, orderItem2.ProductPrice);

            Assert.Equal((1 * 1000.0f) + (3 * 75.0f), order.TotalAmount); 
            _mockProductRepository.Verify(repo => repo.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);
        }

        [Fact]
        public async Task Invoke_HandlesProductNotFound_SettingPriceToZero()
        {
            
            var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Price = 1000.0f }
        };
            _mockProductRepository.Setup(repo => repo.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(products);

            var model = new OrderUpdateModel
            {
                CustomerId = 102,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 1 },
                new OrderItemInsertModel { ProductId = 99, Quantity = 5 } 
            }
            };

            
            var order = await _orderAdaptor.Invoke(model, BusinessSetOperation.Create);

            
            Assert.NotNull(order);
            Assert.Equal(2, order.OrderItems.Count);

            var existingProductItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 1);
            Assert.NotNull(existingProductItem);
            Assert.Equal(1000.0f, existingProductItem.ProductPrice);

            var nonExistingProductItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == 99);
            Assert.NotNull(nonExistingProductItem);
            Assert.Equal(0.0f, nonExistingProductItem.ProductPrice); 
            Assert.Equal((1 * 1000.0f) + (5 * 0.0f), order.TotalAmount); 
        }

        [Fact]
        public async Task Invoke_HandlesEmptyProductListFromRepository()
        {
            
            _mockProductRepository.Setup(repo => repo.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(new List<Product>()); 

            var model = new OrderUpdateModel
            {
                CustomerId = 103,
                Items = new List<OrderItemInsertModel>
            {
                new OrderItemInsertModel { ProductId = 1, Quantity = 2 }
            }
            };

            
            var order = await _orderAdaptor.Invoke(model, BusinessSetOperation.Create);

            
            Assert.NotNull(order);
            Assert.Single(order.OrderItems);
            Assert.Equal(0.0f, order.OrderItems.First().ProductPrice);
            Assert.Equal(0.0f, order.TotalAmount);
        }

    }
}
