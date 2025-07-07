using business.validata.com.Interfaces.Utils;
using business.validata.com.Utils;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Validators;
using Moq;
using System.Linq.Expressions;


namespace business.validata.test.Utils
{
    public class StringFieldValidationTests
    {
        private readonly Mock<ICommandRepository<Product>> _mockRepository;
        private readonly Mock<IGenericLambdaExpressions> _mockGenericLambdaExpressions;
        private readonly StringFieldValidation<Product> _stringFieldValidation;

        public StringFieldValidationTests()
        {
            _mockRepository = new Mock<ICommandRepository<Product>>();
            _mockGenericLambdaExpressions = new Mock<IGenericLambdaExpressions>();
            _stringFieldValidation = new StringFieldValidation<Product>(
                _mockRepository.Object,
                _mockGenericLambdaExpressions.Object
            );
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StringFieldValidation<Product>(
                null!, _mockGenericLambdaExpressions.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfGenericLambdaExpressionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StringFieldValidation<Product>(
                _mockRepository.Object, null!));
        }

        [Fact]
        public async Task InvokeAsync_ReturnsEmptyMessage_WhenValueIsEmpty()
        {
            var entity = new Product { Name = "" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                
            };

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.Equal("Field Name is required", result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsRegexMessage_WhenCheckRegexIsTrueAndValueDoesNotMatch()
        {
            var entity = new Product { Name = "ABC-123" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                CheckRegex = true,
                Regex = "^[A-Z]{3}$"
            };

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.Equal("Field Name is invalid", result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsNull_WhenCheckRegexIsTrueAndValueMatches()
        {
            var entity = new Product { Name = "ABC" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                CheckRegex = true,
                Regex = "^[A-Z]{3}$",
            };

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.Null(result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsUniqueMessage_WhenCheckUniqueIsTrueAndEntityExists()
        {
            var entity = new Product { ProductId = 1, Name = "Existing Name" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = "Name",
                CheckUnique = true,
                Ids = new List<int> { entity.ProductId }
            };
            var existingEntity = new Product { ProductId = 2, Name = "Existing Name" }; 

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByUniqueValue(
                It.IsAny<Product>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<int>>()))
                .Returns<Product, string, string, List<int>>((e, f, v, ids) => (x => x.Name == v && !ids!.Contains(x.ProductId)));

            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(existingEntity); 

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.StartsWith("Field Name value belongs to an other", result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Once);
            _mockGenericLambdaExpressions.Verify(gle => gle.GetEntityByUniqueValue(
                entity, "Name", "Existing Name", stringField.Ids), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsNull_WhenCheckUniqueIsTrueAndEntityDoesNotExist()
        {
            var entity = new Product { ProductId = 1, Name = "New Name" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = "Name",
                CheckUnique = true,
               Ids = new List<int> { entity.ProductId }
            };

            _mockGenericLambdaExpressions.Setup(gle => gle.GetEntityByUniqueValue(
                It.IsAny<Product>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<int>>()))
                .Returns<Product, string, string, List<int>>((e, f, v, ids) => (x => x.Name == v && !ids!.Contains(x.ProductId)));

            _mockRepository.Setup(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product?)null); 

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.Null(result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Once);
            _mockGenericLambdaExpressions.Verify(gle => gle.GetEntityByUniqueValue(
                entity, "Name", "New Name", stringField.Ids), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ReturnsNull_WhenNoValidationChecksAreEnabled()
        {
            var entity = new Product { Name = "Valid Name" };
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = "Name",
                CheckRegex = false,
                CheckUnique = false
            };

            var result = await _stringFieldValidation.InvokeAsnc(stringField);

            Assert.Null(result);
            _mockRepository.Verify(r => r.GetEntityAsync(It.IsAny<Expression<Func<Product, bool>>>()), Times.Never);
        }

        
    }
}
