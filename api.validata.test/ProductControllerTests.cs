using api.validata.com.Controllers;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Adaptors;
using Microsoft.Extensions.Logging;
using model.validata.com;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using Moq;

namespace api.validata.test
{


    public class ProductControllerTests
    {
        private readonly Mock<ILogger<ProductController>> _loggerMock = new();
        private readonly Mock<IProductCommandBusiness> _commandMock = new();
        private readonly Mock<IProductQueryBusiness> _queryMock = new();
        private readonly Mock<IProductAdaptor> _adaptorMock = new();
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            

            _controller = new ProductController(
                _loggerMock.Object,
                _commandMock.Object,
                _queryMock.Object,
                _adaptorMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductController(
                    null!,
                    _commandMock.Object,
                    _queryMock.Object,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfCommandBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductController(
                    _loggerMock.Object,
                    null!,
                    _queryMock.Object,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfQueryBusinessIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductController(
                    _loggerMock.Object,
                    _commandMock.Object,
                    null!,
                    _adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductController(
                    _loggerMock.Object,
                    _commandMock.Object,
                    _queryMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
        {
            var controller = new ProductController(
                _loggerMock.Object,
                _commandMock.Object,
                _queryMock.Object,
                _adaptorMock.Object);

            Assert.NotNull(controller);
        }

        [Fact]
        public async Task List_ReturnsProductList_WhenSuccess()
        {
            var expected = new QueryResult<IEnumerable<ProductModel>>
            {
                Success = true,
                Data = new List<ProductModel> { new() }
            };

            _queryMock.Setup(q => q.ListAsync(It.IsAny<PaginationRequest>())).ReturnsAsync(expected);

            var result = await _controller.List(1, 10);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Insert_ReturnsSuccess_WhenProductCreated()
        {
            var request = new ProductBaseModel { Name = "Product", Price = 10m };
            var product = new Product();
            var expected = new CommandResult<ProductModel> { Success = true };

            _adaptorMock.Setup(a => a.Invoke(It.IsAny<ProductModel>())).Returns(product);
            _commandMock.Setup(c => c.InvokeAsync(product, BusinessSetOperation.Create)).ReturnsAsync(expected);

            var result = await _controller.Insert(request);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Update_ReturnsSuccess_WhenProductUpdated()
        {
            var request = new ProductModel { ProductId = 1, Name = "Updated", Price = 20m };
            var product = new Product();
            var expected = new CommandResult<ProductModel> { Success = true };

            _adaptorMock.Setup(a => a.Invoke(request)).Returns(product);
            _commandMock.Setup(c => c.InvokeAsync(product, BusinessSetOperation.Update)).ReturnsAsync(expected);

            var result = await _controller.Update(request);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Delete_ReturnsSuccess_WhenDeleted()
        {
            var expected = new CommandResult<Product> { Success = true };

            _commandMock.Setup(c => c.DeleteAsync(1)).ReturnsAsync(expected);

            var result = await _controller.Delete(1);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Get_ReturnsProduct_WhenFound()
        {
            var expected = new QueryResult<ProductModel?> { Success = true, Data = new ProductModel() };

            _queryMock.Setup(q => q.GetAsync(1)).ReturnsAsync(expected);

            var result = await _controller.Get(1);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }

}
