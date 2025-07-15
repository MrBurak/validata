using business.validata.com.Utils;
using model.validata.com.Entities;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Product;


namespace business.validata.test.Utils
{
    public class StringFieldValidationTests
    {

        private readonly StringFieldValidation<Product> _stringFieldValidation;

        public StringFieldValidationTests()
        {
            
            _stringFieldValidation = new StringFieldValidation<Product>();
        }

        

       
        [Fact]
        public void Invoke_ReturnsRegexMessage_WhenCheckRegexIsTrueAndValueDoesNotMatch()
        {
            var entity = new Product(0, new ProductName("ABC-123"), new ProductPrice(0));
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                CheckRegex = true,
                Regex = "^[A-Z]{3}$"
            };

            var result = _stringFieldValidation.Invoke(stringField);

            Assert.Equal("Field Name is invalid", result);
        }

        [Fact]
        public void Invoke_ReturnsNull_WhenCheckRegexIsTrueAndValueMatches()
        {
            var entity = new Product(0, new ProductName("ABC"), new ProductPrice(0));
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                CheckRegex = true,
                Regex = "^[A-Z]{3}$",
            };

            var result = _stringFieldValidation.Invoke(stringField);

            Assert.Null(result);
        }

        

        [Fact]
        public void Invoke_ReturnsNull_WhenNoValidationChecksAreEnabled()
        {
            var entity = new Product(0, new ProductName("Valid Name"), new ProductPrice(0));
            var stringField = new StringField<Product>
            {
                Entity = entity,
                Field = nameof(Product.Name),
                CheckRegex = false,
            };

            var result = _stringFieldValidation.Invoke(stringField);

            Assert.Null(result);
        }

        
    }
}
