using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using data.validata.com.Entities;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using Moq;
using test.utils;

namespace api.validata.test
{
    public class ProductControllerTests
    {
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly Mock<IProductCommandBusiness> _mockCommandBusiness;
        private readonly Mock<IProductQueryBusiness> _mockQueryBusiness;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockLogger = new Mock<ILogger<ProductController>>();
            _mockCommandBusiness = new Mock<IProductCommandBusiness>();
            _mockQueryBusiness = new Mock<IProductQueryBusiness>();
            _controller = new ProductController(
                _mockLogger.Object,
                _mockCommandBusiness.Object,
                _mockQueryBusiness.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductController(
                null!, _mockCommandBusiness.Object, _mockQueryBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductController(
                _mockLogger.Object, null!, _mockQueryBusiness.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductController(
                _mockLogger.Object, _mockCommandBusiness.Object, null!));
        }

        [Fact]
        public async Task List_ReturnsQueryResultFromQueryBusiness()
        {
            var expectedResult = new QueryResult<IEnumerable<ProductModel>>
            {
                Success = true,
                Data = new List<ProductModel>
            {
                new ProductModel { ProductId = 1, Name = "Laptop", Price = 1200.00f },
                new ProductModel { ProductId = 2, Name = "Mouse", Price = 25.00f }
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
            var insertModel = new ProductBaseModel { Name = "New Product", Price = 99.99f };
            var expectedCommandResult = new CommandResult<ProductModel>
            {
                Success = true,
                Data = new ProductModel { ProductId = 1, Name = "New Product", Price = 99.99f }
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(It.IsAny<Product>(), BusinessSetOperation.Create))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Insert(insertModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(
                It.Is<Product>(p => p.Name == insertModel.Name && p.Price == insertModel.Price),
                BusinessSetOperation.Create), Times.Once);
        }

        [Fact]
        public async Task Update_MapsRequestAndInvokesCommandBusiness()
        {
            var updateModel = new ProductModel { ProductId = 1, Name = "Updated Product", Price = 199.99f };
            var expectedCommandResult = new CommandResult<ProductModel>
            {
                Success = true,
                Data = new ProductModel { ProductId = 1, Name = "Updated Product", Price = 199.99f }
            };

            _mockCommandBusiness.Setup(b => b.InvokeAsync(It.IsAny<Product>(), BusinessSetOperation.Update))
                .ReturnsAsync(expectedCommandResult);

            var result = await _controller.Update(updateModel);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.InvokeAsync(
                It.Is<Product>(p => p.ProductId == updateModel.ProductId && p.Name == updateModel.Name && p.Price == updateModel.Price),
                BusinessSetOperation.Update), Times.Once);
        }

        [Fact]
        public async Task Delete_InvokesCommandBusinessDelete()
        {
            int productIdToDelete = 1;
            var expectedCommandResult = new CommandResult<Product>
            {
                Success = true,
                Data = new Product { ProductId = productIdToDelete, Name = "Deleted Product" }
            };

            _mockCommandBusiness.Setup(b => b.DeleteAsync(productIdToDelete)).ReturnsAsync(expectedCommandResult);

            var result = await _controller.Delete(productIdToDelete);

            Assert.Equal(expectedCommandResult, result);
            _mockCommandBusiness.Verify(b => b.DeleteAsync(productIdToDelete), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsQueryResultFromQueryBusiness()
        {
            int productIdToGet = 1;
            var expectedResult = new QueryResult<ProductModel?>
            {
                Success = true,
                Data = new ProductModel { ProductId = productIdToGet, Name = "Retrieved Product", Price = 500.00f }
            };

            _mockQueryBusiness.Setup(b => b.GetAsync(productIdToGet)).ReturnsAsync(expectedResult);

            var result = await _controller.Get(productIdToGet);

            Assert.Equal(expectedResult, result);
            _mockQueryBusiness.Verify(b => b.GetAsync(productIdToGet), Times.Once);
        }
    }
}
