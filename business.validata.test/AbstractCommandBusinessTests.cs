using business.validata.com;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace business.validata.test
{
    
    public class AbstractCommandBusinessTests
    {
        private readonly Mock<IGenericValidation<Product>> _mockGenericValidation;
        private readonly Mock<ICommandRepository<Product>> _mockRepository;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly AbstractCommandBusiness<Product> _commandBusiness;

        public AbstractCommandBusinessTests()
        {
            _mockGenericValidation = new Mock<IGenericValidation<Product>>();
            _mockRepository = new Mock<ICommandRepository<Product>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();

            _commandBusiness = new Mock<AbstractCommandBusiness<Product>>(
                _mockGenericValidation.Object,
                _mockRepository.Object,
                _mockGenericLambdaExpressions.Object
            )
            { CallBase = true }.Object;
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericValidationIsNull()
        {
            IGenericValidation<Product> nullValidation = null!;
            Assert.Throws<TargetInvocationException>(() => new Mock<AbstractCommandBusiness<Product>>(
                nullValidation, _mockRepository.Object, _mockGenericLambdaExpressions.Object).Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            ICommandRepository<Product> nullRepository = null!;
            Assert.Throws<TargetInvocationException>(() => new Mock<AbstractCommandBusiness<Product>>(
                _mockGenericValidation.Object, nullRepository, _mockGenericLambdaExpressions.Object).Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericLambdaExpressionsIsNull()
        {
            IGenericLambdaExpressions nullExpressions = null!;
            Assert.Throws<TargetInvocationException>(() => new Mock<AbstractCommandBusiness<Product>>(
                _mockGenericValidation.Object, _mockRepository.Object, nullExpressions).Object);
        }

        [Fact]
        public async Task DeleteAllAsync_UpdatesEntitiesAndReturnsSuccess()
        {
            Expression<Func<Product, bool>> testExpression = x => x.ProductId > 0;
            _mockRepository.Setup(r => r.UpdateAsync(testExpression, It.IsAny<List<Action<Product>>>()))
                .Returns(Task.CompletedTask);

            var result = await _commandBusiness.DeleteAllAsync(testExpression);

            _mockRepository.Verify(r => r.UpdateAsync(testExpression, It.IsAny<List<Action<Product>>>()), Times.Once);
            Assert.True(result.Success);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task DeleteAllAsync_HandlesRepositoryExceptionAndReturnsFailure()
        {
            Expression<Func<Product, bool>> testExpression = x => x.ProductId > 0;
            var expectedExceptionMessage = "Database error";
            _mockRepository.Setup(r => r.UpdateAsync(testExpression, It.IsAny<List<Action<Product>>>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _commandBusiness.DeleteAllAsync(testExpression);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
        }

        [Fact]
        public async Task DeleteAsync_WhenEntityDoesNotExist_NoActionNeeded()
        {
            int idToDelete = 1;
            _mockGenericValidation.Setup(gv => gv.Exists(idToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Product> {  Entity = null });

            var result = await _commandBusiness.DeleteAsync(idToDelete);

            Assert.False(result.Success);

        }

        [Fact]
        public async Task DeleteAsync_WhenEntityExists_UpdatesEntityAndReturnsSuccess()
        {
            int idToDelete = 1;
            var existingEntity = new Product { ProductId = idToDelete };
            Expression<Func<Product, bool>> expectedExpression = x => x.ProductId == idToDelete;

            _mockGenericValidation.Setup(gv => gv.Exists(idToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Product> { Entity = existingEntity });
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityById<Product>(idToDelete))
                .Returns(expectedExpression);
            _mockRepository.Setup(r => r.UpdateAsync(expectedExpression, It.IsAny<List<Action<Product>>>()))
                .Returns(Task.CompletedTask);

            var result = await _commandBusiness.DeleteAsync(idToDelete);

            _mockRepository.Verify(r => r.UpdateAsync(expectedExpression, It.IsAny<List<Action<Product>>>()), Times.Once);
            Assert.True(result.Success);
            Assert.Empty(result.Validations);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task DeleteAsync_HandlesRepositoryExceptionAndReturnsFailure()
        {
            int idToDelete = 1;
            var existingEntity = new Product { ProductId = idToDelete };
            var expectedExceptionMessage = "Delete failed";
            Expression<Func<Product, bool>> expectedExpression = x => x.ProductId == idToDelete;

            _mockGenericValidation.Setup(gv => gv.Exists(idToDelete, BusinessSetOperation.Delete))
                .ReturnsAsync(new ExistsResult<Product> { Entity = existingEntity });
            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityById<Product>(idToDelete))
                .Returns(expectedExpression);
            _mockRepository.Setup(r => r.UpdateAsync(expectedExpression, It.IsAny<List<Action<Product>>>()))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            var result = await _commandBusiness.DeleteAsync(idToDelete);

            Assert.False(result.Success);
            Assert.Equal(expectedExceptionMessage, result.Exception);
            Assert.Empty(result.Validations);
        }

        [Fact]
        public async Task InvokeAsync_CreateOperation_CallsAddAsyncAndReturnsEntity()
        {
            var requestEntity = new Product { ProductId = 0, Name = "New Entity" };
            var returnedEntity = new Product { ProductId = 1, Name = "New Entity" };
            List<Action<Product>> properties = new();

            _mockRepository.Setup(r => r.AddAsync(requestEntity)).ReturnsAsync(returnedEntity);

            var result = await _commandBusiness.InvokeAsync(new Product(), requestEntity, BusinessSetOperation.Create, properties);

            _mockRepository.Verify(r => r.AddAsync(requestEntity), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<List<Action<Product>>>()), Times.Never);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never);
            Assert.Same(returnedEntity, result);
        }

        [Fact]
        public async Task InvokeAsync_UpdateOperation_CallsUpdateAndGetEntityAsyncAndReturnsEntity()
        {
            var entityToUpdate = new Product { ProductId = 1, Name = "Old Name" };
            var requestEntity = new Product { ProductId = 1, Name = "New Name" };
            var updatedEntity = new Product { ProductId = 1, Name = "New Name", LastModifiedTimeStamp = DateTime.UtcNow };
            List<Action<Product>> properties = new();
            Expression<Func<Product, bool>> expectedQuery = x => x.ProductId == 1;

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByPrimaryKey(entityToUpdate))
                .Returns(expectedQuery);
            _mockRepository.Setup(r => r.UpdateAsync(expectedQuery, properties))
                .Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetEntityAsync(expectedQuery))
                .ReturnsAsync(updatedEntity);

            var result = await _commandBusiness.InvokeAsync(entityToUpdate, requestEntity, BusinessSetOperation.Update, properties);

            _mockGenericLambdaExpressions.Verify(gle => gle.GetEntityByPrimaryKey(entityToUpdate), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(expectedQuery, properties), Times.Once);
            _mockRepository.Verify(r => r.GetEntityAsync(expectedQuery), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
            Assert.Same(updatedEntity, result);
        }

        [Fact]
        public async Task InvokeAsync_HandlesExceptionAndRethrows()
        {
            var requestEntity = new Product { ProductId = 0, Name = "New Entity" };
            List<Action<Product>> properties = new();
            var expectedException = new InvalidOperationException("Something went wrong");

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).ThrowsAsync(expectedException);

            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _commandBusiness.InvokeAsync(new Product(), requestEntity, BusinessSetOperation.Create, properties));

            Assert.Same(expectedException, thrownException);
        }
    }
}
