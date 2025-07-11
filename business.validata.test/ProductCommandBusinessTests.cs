using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using Microsoft.Extensions.Logging;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class ProductCommandBusinessTests
    {
        private readonly Mock<IProductValidation> _mockProductValidation=new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork=new();
        private readonly Mock<IGenericValidation<Product>> _mockGenericValidation = new();
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions = new();
        private readonly Mock<ILogger<ProductCommandBusiness>> _mockLogger = new();
        private readonly ProductCommandBusiness _productCommandBusiness;

        public ProductCommandBusinessTests()
        {
           

           
           


            _productCommandBusiness = new ProductCommandBusiness(
                _mockProductValidation.Object,
                _mockUnitOfWork.Object,
                _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object,
                _mockLogger.Object
            );

            _mockProductValidation.Setup(v => v.InvokeAsync(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ValidationResult<Product> { Entity = new Product { ProductId = 1, Name = "Validated Product", Price = 100f } });

          
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByPrimaryKey(It.IsAny<Product>()))
                .Returns<Product>(p => prod => prod.ProductId == p.ProductId);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductCommandBusiness(
                null!, _mockUnitOfWork.Object, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object, _mockLogger.Object));
        }

        [Fact]
        public async Task InvokeAsync_ValidationFails_ReturnsErrors()
        {
            var err = new ValidationResult<Product>
            {
                
            };
            err.AddError("Product name is too short");
            var product = new Product { Name = "Invalid Product" };
            _mockProductValidation.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Create))
                .ReturnsAsync(err);

            var result = await _productCommandBusiness.InvokeAsync(product, BusinessSetOperation.Create);

            Assert.False(result.Success);
            Assert.Contains("Product name is too short", result.Validations);
            Assert.Null(result.Data);
            
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_CallsAddAsyncAndReturnsSuccess()
        {
            var productToCreate = new Product { Name = "New Product", Price = 200f };
            var validatedProduct = new Product { ProductId = 0, Name = "New Product", Price = 200f }; 

            _mockProductValidation.Setup(v => v.InvokeAsync(productToCreate, BusinessSetOperation.Create))
                .ReturnsAsync(new ValidationResult<Product> { Entity = validatedProduct });
         

            var result = await _productCommandBusiness.InvokeAsync(productToCreate, BusinessSetOperation.Create);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data!.ProductId);
            Assert.Equal("New Product", result.Data.Name);
            Assert.Equal(200f, result.Data.Price);
            
        }

        [Fact]
        public async Task InvokeAsync_UpdateOperation_CallsUpdateAndGetEntityAsyncAndReturnsSuccess()
        {
            var productToUpdate = new Product { ProductId = 1, Name = "Updated Product Name", Price = 150f };
            var validatedExistingProduct = new Product { ProductId = 1, Name = "Original Name", Price = 100f }; 

            _mockProductValidation.Setup(v => v.InvokeAsync(productToUpdate, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Product> { Entity = validatedExistingProduct });


            var result = await _productCommandBusiness.InvokeAsync(productToUpdate, BusinessSetOperation.Update);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data!.ProductId);
            Assert.Equal("Updated Product Name", result.Data.Name);
            Assert.Equal(150f, result.Data.Price);
            
        }

        [Fact]
        public async Task InvokeAsync_HandlesException_ReturnsFailure()
        {
            var product = new Product { ProductId = 1, Name = "Error Product", Price = 10f };
            var expectedExceptionMessage = "Database connection lost during update";

            _mockProductValidation.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Product> { Entity = new Product { ProductId = 1 } });


            var result = await _productCommandBusiness.InvokeAsync(product, BusinessSetOperation.Update);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Data);
        }
    }
}
