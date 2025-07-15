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
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test
{
    public class OrderCommandBusinessTests
    {
        private readonly Mock<IOrderValidation> orderValidationMock = new();
        private readonly Mock<IOrderAdaptor> orderAdaptorMock = new();
        private readonly Mock<ICommandRepository<Order>> repositoryMock = new();
        private readonly Mock<IUnitOfWork> unitOfWorkMock = new();
        private readonly Mock<IGenericValidation<Order>> genericValidationMock = new();
        private readonly Mock<IGenericLambdaExpressions> lambdaMock = new();
        private readonly Mock<ILogger<OrderCommandBusiness>> loggerMock = new();

        private readonly OrderCommandBusiness sut;

        public OrderCommandBusinessTests()
        {
            sut = new OrderCommandBusiness
                (
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    unitOfWorkMock.Object,
                    orderAdaptorMock.Object,
                    loggerMock.Object
                );
        }
        [Fact]
        public void Constructor_ThrowsIfValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    null!,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    unitOfWorkMock.Object,
                    orderAdaptorMock.Object,
                    loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfGenericLambdaExpressionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    null!,
                    unitOfWorkMock.Object,
                    orderAdaptorMock.Object,
                    loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfGenericValidationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    null!,
                    lambdaMock.Object,
                    unitOfWorkMock.Object,
                    orderAdaptorMock.Object,
                    loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfOrderAdaptorIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    unitOfWorkMock.Object,
                    null!,
                    loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfUnitOfWorkIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    null!,
                    orderAdaptorMock.Object,
                    loggerMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new OrderCommandBusiness(
                    orderValidationMock.Object,
                    repositoryMock.Object,
                    genericValidationMock.Object,
                    lambdaMock.Object,
                    unitOfWorkMock.Object,
                    orderAdaptorMock.Object,
                    null!));
        }

        [Fact]
        public void Constructor_Succeeds_WhenAllDependenciesAreValid()
        {
            var sut = new OrderCommandBusiness(
                orderValidationMock.Object,
                repositoryMock.Object,
                genericValidationMock.Object,
                lambdaMock.Object,
                unitOfWorkMock.Object,
                orderAdaptorMock.Object,
                loggerMock.Object);

            Assert.NotNull(sut);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnValidationError_WhenExistsFails()
        {
            genericValidationMock.Setup(x => x.Exists(1, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Order> { Entity = null });

            var result = await sut.DeleteAsync(1);

            result.Success.Should().BeFalse();
            result.Validations.Should().Contain("No record found");
        }

        

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse()
        {
            var expr = (Expression<Func<Order, bool>>)(x => x.OrderId == 1);
            lambdaMock.Setup(x => x.GetEntityById<Order>(1)).Returns(expr);

            genericValidationMock.Setup(x => x.Exists(1, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Order>());

            unitOfWorkMock.Setup(x => x.orders.DeleteAsync(expr)).ThrowsAsync(new Exception(""));

            var result = await sut.DeleteAsync(1);

            result.Success.Should().BeFalse();
        }

        

        [Fact]
        public async Task DeleteOrdersAsync_ShouldCatchException()
        {
            unitOfWorkMock.Setup(x => x.orders.DeleteAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .ThrowsAsync(new Exception("delete fail"));

            var result = await sut.InvokeDeleteOrdersForTest(x => true);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("delete fail");
        }

       

        [Fact]
        public async Task DeleteOrderItemsAsync_ShouldReturnException()
        {
            unitOfWorkMock.Setup(x => x.orderItems.DeleteAsync(It.IsAny<Expression<Func<OrderItem, bool>>>()))
                .ThrowsAsync(new Exception("order item fail"));

            var result = await sut.InvokeDeleteOrderItemsForTest(x => true);

            result.Success.Should().BeFalse();
            result.Exception.Should().Be("order item fail");
        }
    }

}
