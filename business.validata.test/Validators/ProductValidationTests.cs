using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test.Validators
{
    public class ProductValidationTests
    {
        private readonly Mock<IGenericValidation<Product>> _mockGenericValidation;
        private readonly Mock<ICommandRepository<Product>> _mockRepository;
        private readonly ProductValidation _productValidation;

        public ProductValidationTests()
        {
            _mockGenericValidation = new Mock<IGenericValidation<Product>>();
            _mockRepository = new Mock<ICommandRepository<Product>>();
            _productValidation = new ProductValidation(_mockGenericValidation.Object, _mockRepository.Object);

            
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Product> { Entity = new Product() });

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                    It.IsAny<Product>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<string>()))
                .ReturnsAsync((string?)null); 

            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product>());
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationIsNull()
        {
            
            IGenericValidation<Product> nullGenericValidation = null!;
            var mockRepository = new Mock<ICommandRepository<Product>>();

            Assert.Throws<ArgumentNullException>(() => new ProductValidation(nullGenericValidation, mockRepository.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            
            var mockGenericValidation = new Mock<IGenericValidation<Product>>();
            ICommandRepository<Product> nullRepository = null!;

            Assert.Throws<ArgumentNullException>(() => new ProductValidation(mockGenericValidation.Object, nullRepository));
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_SetsProductIdToZero()
        {
            
            var product = new Product { ProductId = 999, Name = "New Product" }; 

            
            await _productValidation.InvokeAsync(product, BusinessSetOperation.Create);

            
            Assert.Equal(0, product.ProductId);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Get)]
        [InlineData(BusinessSetOperation.Delete)]
        public async Task InvokeAsync_NonCreateOperation_DoesNotSetProductIdToZero(BusinessSetOperation operation)
        {
            
            var product = new Product { ProductId = 999, Name = "Existing Product" };

            
            await _productValidation.InvokeAsync(product, operation);

            
            Assert.Equal(999, product.ProductId);
        }

        [Fact]
        public async Task InvokeAsync_ExistsReturnsNullEntity_AddsErrorAndReturnsImmediately()
        {
            
            var product = new Product();
            var expectedErrorCode = "No record found";
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Product> { Entity = null });

            
            var result = await _productValidation.InvokeAsync(product, BusinessSetOperation.Update); 

            
            Assert.False(result.IsValid);
            Assert.Contains(expectedErrorCode, result.Errors);
            Assert.Null(result.Entity); 
            _mockRepository.Verify(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never); 
            _mockGenericValidation.Verify(m => m.ValidateStringField(
                It.IsAny<Product>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()), Times.Never); 
        }

        [Fact]
        public async Task InvokeAsync_ExistsReturnsEntity_SetsEntityInValidationResult()
        {
            
            var product = new Product();
            var existingProduct = new Product { ProductId = 1, Name = "Existing Product" };
            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Product> { Entity = existingProduct });

            
            var result = await _productValidation.InvokeAsync(product, BusinessSetOperation.Update);

            
            Assert.Same(existingProduct, result.Entity);
        }

        [Fact]
        public async Task InvokeAsync_AlwaysRetrievesProductIdsForUniquenessCheck()
        {
            
            var product = new Product { ProductId = 1, Name = "Test" };
            var existingProducts = new List<Product>
        {
            new Product { ProductId = 10, DeletedOn = null },
            new Product { ProductId = 20, DeletedOn = null },
            new Product { ProductId = 30, DeletedOn = DateTime.UtcNow } 
        };
            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(existingProducts);

            
            await _productValidation.InvokeAsync(product, BusinessSetOperation.Create);

            
            _mockRepository.Verify(m => m.GetListAsync(
                It.Is<Expression<Func<Product, bool>>>(exp => exp != null)), Times.Once); 
        }

        [Fact]
        public async Task InvokeAsync_ValidatesProductNameWithUniquenessAndCorrectIds()
        {
            
            var product = new Product { ProductId = 1, Name = "Test Product" };
            var existingProductIds = new List<int> { 10, 20 };
            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product>
                {
                new Product { ProductId = 10, DeletedOn = null },
                new Product { ProductId = 20, DeletedOn = null }
                });

            
            await _productValidation.InvokeAsync(product, BusinessSetOperation.Create);

            
            _mockGenericValidation.Verify(m => m.ValidateStringField(
                product,
                nameof(Product.Name),
                true,  
                true,  
                It.Is<List<int>>(ids => ids.SequenceEqual(existingProductIds)), 
                null), 
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_AllValidationsPass_ReturnsValidResult()
        {
            
            var product = new Product { ProductId = 1, Name = "Valid Product" };

            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product }); 

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                    It.IsAny<Product>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<string>()))
                .ReturnsAsync((string?)null);
            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product> { new Product { ProductId = 99, Name = "Other Product" } });

            
            var result = await _productValidation.InvokeAsync(product, BusinessSetOperation.Create);

            
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Equal(product, result.Entity);
        }

        [Fact]
        public async Task InvokeAsync_ProductNameValidationFails_ReturnsInvalidResultWithError()
        {
            
            var product = new Product { ProductId = 1, Name = "" }; 
            var expectedError = "Product Name is required and must be unique.";

            _mockGenericValidation.Setup(m => m.Exists(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product });

            _mockRepository.Setup(m => m.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product>()); 

            _mockGenericValidation.Setup(m => m.ValidateStringField(
                product, nameof(Product.Name), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(expectedError);

            
            var result = await _productValidation.InvokeAsync(product, BusinessSetOperation.Create);

            
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(expectedError, result.Errors);
        }
    }
}
