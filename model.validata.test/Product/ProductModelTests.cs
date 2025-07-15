using model.validata.com.Product;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Product
{
    public class ProductModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void ProductModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new ProductModel
            {
                ProductId = 1,
                Name = "Smartphone",
                Price = 999.99m
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void ProductModel_ProductId_CanBeSetAndGet()
        {
            var model = new ProductModel
            {
                ProductId = 123
            };

            Assert.Equal(123, model.ProductId);
        }

        [Fact]
        public void ProductModel_ProductId_DefaultValueIsZero()
        {
            var model = new ProductModel();

            Assert.Equal(0, model.ProductId);
        }

        [Fact]
        public void ProductModel_InheritedProperties_RequiredAndMaxLengthChecks()
        {
            var model = new ProductModel
            {
                ProductId = 1,
                Name = null, 
                Price = 100.0m
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Name") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void ProductModel_InheritedProperties_RangeChecks()
        {
            var model = new ProductModel
            {
                ProductId = 1,
                Name = "Laptop",
                Price = -50.0m 
            };

            var errors = ValidateModel(model);

            
            Assert.Single(errors);
        }

        [Fact]
        public void ProductModel_MultipleValidationErrors_AcrossBase()
        {
            var model = new ProductModel
            {
                ProductId = 1,
                Name = new string('A', 150), 
                Price = -1.0m
            };

            var errors = ValidateModel(model);

            Assert.Equal(2, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("Name"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Price"));
        }
    }
}
