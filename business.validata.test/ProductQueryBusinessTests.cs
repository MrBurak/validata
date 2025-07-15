using business.validata.com;
using business.validata.com.Interfaces.Adaptors;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Entities;
using model.validata.com.Product;
using model.validata.com.ValueObjects.Product;
using Moq;

namespace business.validata.test
{


    public class ProductQueryBusinessTests
    {
        private readonly Mock<IProductRepository> repoMock = new();
        private readonly Mock<IProductAdaptor> adaptorMock = new();
        private readonly Mock<ILogger<ProductQueryBusiness>> loggerMock = new();

        private readonly ProductQueryBusiness sut;

        public ProductQueryBusinessTests()
        {
            sut = new ProductQueryBusiness(repoMock.Object, loggerMock.Object, adaptorMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsIfRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductQueryBusiness(
                    null!,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductQueryBusiness(
                    repoMock.Object,
                    null!,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductQueryBusiness(
                    repoMock.Object,
                    loggerMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
        {
            var sut = new ProductQueryBusiness(
                repoMock.Object,
                loggerMock.Object,
                adaptorMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnProducts_WhenSuccess()
        {
            
            var pagination = new PaginationRequest(1, 10);
            var products = new List<Product> { new Product(1, new ProductName("A"), new ProductPrice(10)) };
            var models = new List<ProductModel> { new ProductModel { ProductId = 1, Name = "A", Price = 10 } };

            repoMock.Setup(r => r.GetAllAsync(pagination)).ReturnsAsync(products);
            adaptorMock.Setup(a => a.Invoke(products)).Returns(models);

            
            var result = await sut.ListAsync(pagination);

            
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(models);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnError_WhenExceptionThrown()
        {
            
            var pagination = new PaginationRequest(1, 10);
            repoMock.Setup(r => r.GetAllAsync(pagination)).ThrowsAsync(new Exception("DB error"));

            
            var result = await sut.ListAsync(pagination);

            
            result.Success.Should().BeFalse();
            result.Exception.Should().Be("DB error");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnProduct_WhenFound()
        {
            
            var id = 1;
            var product = new Product(id, new ProductName("X"), new ProductPrice(100));
            var model = new ProductModel { ProductId = id, Name = "X", Price = 100 };

            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);
            adaptorMock.Setup(a => a.Invoke(product)).Returns(model);

            
            var result = await sut.GetAsync(id);

            
            result.Success.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNotFound_WhenNull()
        {
            
            int id = 999;
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            
            var result = await sut.GetAsync(id);

            
            result.Success.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Exception.Should().Be("No record found");
        }

        [Fact]
        public async Task GetAsync_ShouldReturnError_WhenExceptionThrown()
        {
            
            int id = 1;
            repoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Get failure"));

            
            var result = await sut.GetAsync(id);

            
            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Get failure");
        }
    }

}
