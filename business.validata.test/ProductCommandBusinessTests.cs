using business.validata.com;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class ProductCommandBusinessTests
    {
        private readonly Mock<IProductValidation> _mockProductValidation;
        private readonly Mock<ICommandRepository<Product>> _mockRepository;
        private readonly Mock<IGenericValidation<Product>> _mockGenericValidation;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly ProductCommandBusiness _productCommandBusiness;

        public ProductCommandBusinessTests()
        {
            _mockProductValidation = new Mock<IProductValidation>();
            _mockRepository = new Mock<ICommandRepository<Product>>();
            _mockGenericValidation = new Mock<IGenericValidation<Product>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();

           
            var abstractCommandBusinessMock = new Mock<AbstractCommandBusiness<Product>>(
                _mockGenericValidation.Object,
                _mockRepository.Object,
                _mockGenericLambdaExpressions.Object
            )
            { CallBase = true }; 


            _productCommandBusiness = new ProductCommandBusiness(
                _mockProductValidation.Object,
                _mockRepository.Object,
                _mockGenericValidation.Object,
                _mockGenericLambdaExpressions.Object
            );

            _mockProductValidation.Setup(v => v.InvokeAsync(It.IsAny<Product>(), It.IsAny<BusinessSetOperation>()))
                .ReturnsAsync(new ValidationResult<Product> { Entity = new Product { ProductId = 1, Name = "Validated Product", Price = 100f } });

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product product) =>
                {
                    product.ProductId = 1; // Simulate ID being set by DB
                    return product;
                });
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<List<Action<Product>>>()))
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new Product { ProductId = 1, Name = "Updated Product", Price = 150f });

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByPrimaryKey(It.IsAny<Product>()))
                .Returns<Product>(p => prod => prod.ProductId == p.ProductId);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProductCommandBusiness(
                null!, _mockRepository.Object, _mockGenericValidation.Object, _mockGenericLambdaExpressions.Object));
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
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<List<Action<Product>>>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_CallsAddAsyncAndReturnsSuccess()
        {
            var productToCreate = new Product { Name = "New Product", Price = 200f };
            var validatedProduct = new Product { ProductId = 0, Name = "New Product", Price = 200f }; 

            _mockProductValidation.Setup(v => v.InvokeAsync(productToCreate, BusinessSetOperation.Create))
                .ReturnsAsync(new ValidationResult<Product> { Entity = validatedProduct });
            _mockRepository.Setup(r => r.AddAsync(validatedProduct))
                .ReturnsAsync(new Product { ProductId = 1, Name = "New Product", Price = 200f }); 

            var result = await _productCommandBusiness.InvokeAsync(productToCreate, BusinessSetOperation.Create);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result!.ProductId);
            Assert.Equal("New Product", result.Result.Name);
            Assert.Equal(200f, result.Result.Price);
            
        }

        [Fact]
        public async Task InvokeAsync_UpdateOperation_CallsUpdateAndGetEntityAsyncAndReturnsSuccess()
        {
            var productToUpdate = new Product { ProductId = 1, Name = "Updated Product Name", Price = 150f };
            var validatedExistingProduct = new Product { ProductId = 1, Name = "Original Name", Price = 100f }; 

            _mockProductValidation.Setup(v => v.InvokeAsync(productToUpdate, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Product> { Entity = validatedExistingProduct });
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new Product { ProductId = 1, Name = "Updated Product Name", Price = 150f }); 

            var result = await _productCommandBusiness.InvokeAsync(productToUpdate, BusinessSetOperation.Update);

            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.NotNull(result.Result);
            Assert.Equal(1, result.Result!.ProductId);
            Assert.Equal("Updated Product Name", result.Result.Name);
            Assert.Equal(150f, result.Result.Price);
            
        }

        [Fact]
        public async Task InvokeAsync_HandlesException_ReturnsFailure()
        {
            var product = new Product { ProductId = 1, Name = "Error Product", Price = 10f };
            var expectedExceptionMessage = "Database connection lost during update";

            _mockProductValidation.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Update))
                .ReturnsAsync(new ValidationResult<Product> { Entity = new Product { ProductId = 1 } });
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<List<Action<Product>>>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _productCommandBusiness.InvokeAsync(product, BusinessSetOperation.Update);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Null(result.Result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<List<Action<Product>>>()), Times.Once);
        }
    }
}
