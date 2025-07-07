using business.validata.com.Interfaces.Utils;
using business.validata.com.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;

namespace business.validata.test.Validators
{
    public class GenericValidationTests
    {
        private readonly Mock<ICommandRepository<Customer>> _mockRepository;
        private readonly Mock<IGenericLambdaExpressions> _mockLambdaExpressions;
        private readonly Mock<IStringFieldValidation<Customer>> _mockStringFieldValidation;
        private readonly GenericValidation<Customer> _genericValidation;

        public GenericValidationTests()
        {
            _mockRepository = new Mock<ICommandRepository<Customer>>();
            _mockLambdaExpressions = new Mock<IGenericLambdaExpressions>();
            _mockStringFieldValidation = new Mock<IStringFieldValidation<Customer>>();
            _genericValidation = new GenericValidation<Customer>(
                _mockRepository.Object,
                _mockLambdaExpressions.Object,
                _mockStringFieldValidation.Object
            );


            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer?)null); 
            _mockLambdaExpressions.Setup(l => l.GetEntityByPrimaryKey(It.IsAny<Customer>()))
                .Returns(x => true);

            _mockLambdaExpressions.Setup(l => l.GetEntityById<Customer>(It.IsAny<int>()))
                .Returns(x => true);

            _mockStringFieldValidation.Setup(s => s.InvokeAsnc(It.IsAny<StringField<Customer>>()))
                .ReturnsAsync((string?)null); 
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            
            ICommandRepository<Customer> nullRepository = null!;

            Assert.Throws<ArgumentNullException>(() => new GenericValidation<Customer>(
                nullRepository, _mockLambdaExpressions.Object, _mockStringFieldValidation.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfLambdaExpressionsIsNull()
        {
            
            IGenericLambdaExpressions nullLambdaExpressions = null!;

            Assert.Throws<ArgumentNullException>(() => new GenericValidation<Customer>(
                _mockRepository.Object, nullLambdaExpressions, _mockStringFieldValidation.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfStringFieldValidationIsNull()
        {
            
            IStringFieldValidation<Customer> nullStringFieldValidation = null!;
            Assert.Throws<ArgumentNullException>(() => new GenericValidation<Customer>(
                _mockRepository.Object, _mockLambdaExpressions.Object, nullStringFieldValidation));
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task ExistsByEntity_ReturnsExistsResultWithEntity_WhenEntityFoundAndOperationIsRelevant(BusinessSetOperation operation)
        {
            
            var customer = new Customer { CustomerId = 1, FirstName = "John" };
            _mockLambdaExpressions.Setup(l => l.GetEntityByPrimaryKey(customer))
                .Returns(c => c.CustomerId == customer.CustomerId);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(customer);

            
            var result = await _genericValidation.Exists(customer, operation);

            
            Assert.NotNull(result);
            Assert.Same(customer, result!.Entity);
            _mockLambdaExpressions.Verify(l => l.GetEntityByPrimaryKey(customer), Times.Once);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task ExistsByEntity_ReturnsExistsResultWithNullEntity_WhenEntityNotFoundAndOperationIsRelevant(BusinessSetOperation operation)
        {
            
            var customer = new Customer { CustomerId = 1, FirstName = "John" };
            _mockLambdaExpressions.Setup(l => l.GetEntityByPrimaryKey(customer))
                .Returns(c => c.CustomerId == customer.CustomerId);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer?)null);

            
            var result = await _genericValidation.Exists(customer, operation);

            
            Assert.NotNull(result);
            Assert.Null(result!.Entity);
            _mockLambdaExpressions.Verify(l => l.GetEntityByPrimaryKey(customer), Times.Once);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Create)]
        public async Task ExistsByEntity_ReturnsNull_WhenOperationIsNotRelevant(BusinessSetOperation operation)
        {
            
            var customer = new Customer { CustomerId = 1, FirstName = "John" };

            
            var result = await _genericValidation.Exists(customer, operation);

            
            Assert.Null(result);
            _mockLambdaExpressions.Verify(l => l.GetEntityByPrimaryKey(It.IsAny<Customer>()), Times.Never);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Never);
        }

        
        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task ExistsById_ReturnsExistsResultWithEntity_WhenEntityFoundAndOperationIsRelevant(BusinessSetOperation operation)
        {
            
            int customerId = 123;
            var customer = new Customer { CustomerId = customerId, FirstName = "Jane" };
            _mockLambdaExpressions.Setup(l => l.GetEntityById<Customer>(customerId))
                .Returns(c => c.CustomerId == customerId);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(customer);

            
            var result = await _genericValidation.Exists(customerId, operation);

            
            Assert.NotNull(result);
            Assert.Same(customer, result!.Entity);
            _mockLambdaExpressions.Verify(l => l.GetEntityById<Customer>(customerId), Times.Once);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Update)]
        [InlineData(BusinessSetOperation.Delete)]
        [InlineData(BusinessSetOperation.Get)]
        public async Task ExistsById_ReturnsExistsResultWithNullEntity_WhenEntityNotFoundAndOperationIsRelevant(BusinessSetOperation operation)
        {
            
            int customerId = 123;
            _mockLambdaExpressions.Setup(l => l.GetEntityById<Customer>(customerId))
                .Returns(c => c.CustomerId == customerId);
            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync((Customer?)null);

            
            var result = await _genericValidation.Exists(customerId, operation);

            
            Assert.NotNull(result);
            Assert.Null(result!.Entity);
            _mockLambdaExpressions.Verify(l => l.GetEntityById<Customer>(customerId), Times.Once);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData(BusinessSetOperation.Create)]
        public async Task ExistsById_ReturnsNull_WhenOperationIsNotRelevant(BusinessSetOperation operation)
        {
            
            int customerId = 123;

            
            var result = await _genericValidation.Exists(customerId, operation);

            
            Assert.Null(result);
            _mockLambdaExpressions.Verify(l => l.GetEntityById<Customer>(It.IsAny<int>()), Times.Never);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Never);
        }

        

       
    }
}
