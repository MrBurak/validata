using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using model.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;
using FluentAssertions;
using model.validata.com.ValueObjects.Product;
using System.Net.NetworkInformation;

namespace business.validata.test.Validators
{

    public class ProductValidationTests
    {
        private readonly Mock<IGenericValidation<Product>> genericValidationMock = new();
        private readonly Mock<ICommandRepository<Product>> repositoryMock = new();

        private readonly ProductValidation sut;

        public ProductValidationTests()
        {
            sut = new ProductValidation(genericValidationMock.Object, repositoryMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenGenericValidationIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ProductValidation(null!, repositoryMock.Object));
            Assert.Contains("genericValidation", ex.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new ProductValidation(genericValidationMock.Object, null!));
            Assert.Contains("repository", ex.ParamName);
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesProvided()
        {
            var sut = new ProductValidation(genericValidationMock.Object, repositoryMock.Object);
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task Should_ReturnError_IfProductEntityDoesNotExist()
        {
            
            var product = CreateProduct();
            genericValidationMock.Setup(g => g.Exists(product, BusinessSetOperation.Update))
                .ReturnsAsync(new ExistsResult<Product> { Entity = null });

            
            var result = await sut.InvokeAsync(product, BusinessSetOperation.Update);

            
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("No record found");
        }

        [Fact]
        public async Task Should_ReturnError_IfNameValidationFails()
        {
            var product = CreateProduct();

            genericValidationMock.Setup(g => g.Exists(product, BusinessSetOperation.Create))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product });

            genericValidationMock.Setup(g => g.ValidateStringField(product, nameof(Product.Name), true, null))
                .Returns("Name is required");

            
            var result = await sut.InvokeAsync(product, BusinessSetOperation.Create);

            
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("Name is required");
        }

        [Fact]
        public async Task Should_ReturnError_IfNameIsNotUnique()
        {
            var product = CreateProduct(productId: 1, name: "Duplicate");

            genericValidationMock.Setup(g => g.Exists(product, BusinessSetOperation.Create))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product });

            genericValidationMock.Setup(g => g.ValidateStringField(product, nameof(Product.Name), true, null))
                .Returns((string?)null);

            repositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product> {
                new Product(2, new ProductName("Duplicate"), new ProductPrice(10)) 
                });

            var result = await sut.InvokeAsync(product, BusinessSetOperation.Create);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("Product name have to be unique");
        }

        [Fact]
        public async Task Should_PassValidation_WhenValid()
        {
            var product = CreateProduct(productId: 1, name: "Unique");

            genericValidationMock.Setup(g => g.Exists(product, BusinessSetOperation.Create))
                .ReturnsAsync(new ExistsResult<Product> { Entity = product });

            genericValidationMock.Setup(g => g.ValidateStringField(product, nameof(Product.Name), true, null))
                .Returns((string?)null);

            repositoryMock.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(new List<Product>()); 

            var result = await sut.InvokeAsync(product, BusinessSetOperation.Create);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        private Product CreateProduct(int productId = 1, string name = "TestProduct")
        {
            return new Product(productId, new ProductName(name), new ProductPrice(9.99m));
        }
    }

}
