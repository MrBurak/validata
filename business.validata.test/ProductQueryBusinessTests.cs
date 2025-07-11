using business.validata.com;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using test.utils;

namespace business.validata.test
{
    public class ProductQueryBusinessTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<ProductQueryBusiness>> _mockLogger;
        private readonly ProductQueryBusiness _productQueryBusiness;

        public ProductQueryBusinessTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductQueryBusiness>>();
            _productQueryBusiness = new ProductQueryBusiness(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ProductQueryBusiness(null!, _mockLogger.Object));
            Assert.Equal("repository", ex.ParamName);
        }

        [Fact]
        public async Task ListAsync_ReturnsAllProductsSuccessfully()
        {
            var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Price = 1200.00f },
            new Product { ProductId = 2, Name = "Mouse", Price = 25.50f }
        };
            _mockRepository.Setup(r => r.GetAllAsync(PaginationUtil.paginationRequest)).ReturnsAsync(products);

            var result = await _productQueryBusiness.ListAsync(PaginationUtil.paginationRequest);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Data);
            var productModels = result.Data!.ToList();
            Assert.Equal(2, productModels.Count);
            Assert.Equal("Laptop", productModels[0].Name);
            Assert.Equal(1200.00f, productModels[0].Price);
            Assert.Equal("Mouse", productModels[1].Name);
            Assert.Equal(25.50f, productModels[1].Price);
            _mockRepository.Verify(r => r.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);
            
        }

        [Fact]
        public async Task ListAsync_HandlesRepositoryException()
        {
            var expectedExceptionMessage = "Database error during ListAsync";
            _mockRepository.Setup(r => r.GetAllAsync(PaginationUtil.paginationRequest)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _productQueryBusiness.ListAsync(PaginationUtil.paginationRequest);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
            _mockRepository.Verify(r => r.GetAllAsync(PaginationUtil.paginationRequest), Times.Once);
            
        }

        [Fact]
        public async Task GetAsync_ReturnsProductSuccessfully()
        {
            int productId = 1;
            var product = new Product { ProductId = productId, Name = "Keyboard", Price = 75.00f};
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _productQueryBusiness.GetAsync(productId);

            Assert.True(result.Success);
            Assert.Null(result.Exception);
            Assert.NotNull(result.Data);
            Assert.Equal(productId, result.Data!.ProductId);
            Assert.Equal("Keyboard", result.Data.Name);
            Assert.Equal(75.00f, result.Data.Price);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
            
        }

        [Fact]
        public async Task GetAsync_ReturnsNoRecordFound_WhenProductDoesNotExist()
        {
            int productId = 99;
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            var result = await _productQueryBusiness.GetAsync(productId);

            Assert.False(result.Success);
            Assert.Equal("No record found", result.Exception);
            Assert.Null(result.Data);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
            
        }

        [Fact]
        public async Task GetAsync_HandlesRepositoryException()
        {
            int productId = 1;
            var expectedExceptionMessage = "Network error during GetAsync";
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _productQueryBusiness.GetAsync(productId);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
          
        }
    }
}
