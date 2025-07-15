using business.validata.com;
using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Adaptors;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Interfaces.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Product;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class ProductCommandBusinessTests
    {
        private readonly Mock<IProductValidation> validationMock = new();
        private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
        private readonly Mock<IGenericLambdaExpressions> lambdaMock = new();
        private readonly Mock<IGenericValidation<Product>> genericValidationMock = new();
        private readonly Mock<ILogger<ProductCommandBusiness>> loggerMock = new();
        private readonly Mock<IProductAdaptor> adaptorMock = new();
        private readonly Mock<ICommandRepository<Product>> productRepoMock = new();

        private readonly ProductCommandBusiness sut;

        public ProductCommandBusinessTests()
        {
            unitOfWorkMock.Setup(u => u.products).Returns(productRepoMock.Object);

            sut = new ProductCommandBusiness(
                validationMock.Object,
                unitOfWorkMock.Object,
                genericValidationMock.Object,
                lambdaMock.Object,
                loggerMock.Object,
                adaptorMock.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsIfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    null!,
                    unitOfWorkMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfUnitOfWorkIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    validationMock.Object,
                    null!,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfGenericLambdaExpressionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    validationMock.Object,
                    unitOfWorkMock.Object,
                    genericValidationMock.Object,
                    null!,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfGenericValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    validationMock.Object,
                    unitOfWorkMock.Object,
                    null!,
                    lambdaMock.Object,
                    loggerMock.Object,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    validationMock.Object,
                    unitOfWorkMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    null!,
                    adaptorMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfProductAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ProductCommandBusiness(
                    validationMock.Object,
                    unitOfWorkMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    loggerMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreProvided()
        {
            var sut = new ProductCommandBusiness(
                validationMock.Object,
                unitOfWorkMock.Object,
                genericValidationMock.Object,
                lambdaMock.Object,
                loggerMock.Object,
                adaptorMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async Task InvokeAsync_Create_ShouldReturnSuccess_WhenValid()
        {

            var product = new Product(0, new ProductName("X"), new ProductPrice(9.99m));
            var validationResult = new ValidationResult<Product>();
            validationMock.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Create)).ReturnsAsync(validationResult);
            productRepoMock.Setup(r => r.AddAsync(product)).ReturnsAsync(product);
            adaptorMock.Setup(a => a.Invoke(product)).Returns(new ProductModel { ProductId = 1 });


            var result = await sut.InvokeAsync(product, BusinessSetOperation.Create);


            result.Success.Should().BeTrue();
            result.Data!.ProductId.Should().Be(1);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnValidationErrors_WhenInvalid()
        {

            var product = new Product(1, new ProductName("Invalid"), new ProductPrice(0));
            var validationResult = new ValidationResult<Product>();
            validationResult.AddError("Invalid name");

            validationMock.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Update)).ReturnsAsync(validationResult);


            var result = await sut.InvokeAsync(product, BusinessSetOperation.Update);


            result.Success.Should().BeFalse();
            result.Validations.Should().Contain("Invalid name");
        }

        [Fact]
        public async Task InvokeAsync_Update_ShouldReturnSuccess_WhenValid()
        {

            var product = new Product(2, new ProductName("Updated"), new ProductPrice(15));
            var query = new object();
            var validationResult = new ValidationResult<Product>();
            Expression<Func<Product, bool>> expression = x => x.DeletedOn == null;
            validationMock.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Update)).ReturnsAsync(validationResult);
            lambdaMock.Setup(l => l.GetEntityByPrimaryKey(product)).Returns(expression);
            productRepoMock.Setup(r => r.UpdateAsync(expression, It.IsAny<List<Action<Product>>>())).Returns(Task.CompletedTask);
            productRepoMock.Setup(r => r.GetEntityAsync(expression)).ReturnsAsync(product);
            adaptorMock.Setup(a => a.Invoke(product)).Returns(new ProductModel { ProductId = 2 });


            var result = await sut.InvokeAsync(product, BusinessSetOperation.Update);


            result.Success.Should().BeTrue();
            result.Data!.ProductId.Should().Be(2);
        }

        [Fact]
        public async Task InvokeAsync_ShouldCatchException_AndReturnFailure()
        {

            var product = new Product(3, new ProductName("Boom"), new ProductPrice(99));
            var validationResult = new ValidationResult<Product>();

            validationMock.Setup(v => v.InvokeAsync(product, BusinessSetOperation.Create)).ReturnsAsync(validationResult);
            productRepoMock.Setup(r => r.AddAsync(product)).ThrowsAsync(new Exception("Insert fail"));


            var result = await sut.InvokeAsync(product, BusinessSetOperation.Create);


            result.Success.Should().BeFalse();
            result.Exception.Should().Be("Insert fail");
        }



    }
}
