using model.validata.com.Product;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Product
{
    public class ProductBaseModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void ProductBaseModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new ProductBaseModel
            {
                Name = "Laptop",
                Price = 1200.50f
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void ProductBaseModel_Name_Required()
        {
            var model = new ProductBaseModel
            {
                Name = null, 
                Price = 50.0f
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Name") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void ProductBaseModel_Name_MaxLength()
        {
            var model = new ProductBaseModel
            {
                Name = new string('A', 129), 
                Price = 50.0f
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Name") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void ProductBaseModel_Price_Required()
        {
          
            var model = new ProductBaseModel
            {
                Name = "Valid Product",
              
                Price = 0.0f 
            };

            var errors = ValidateModel(model);
           
            Assert.DoesNotContain(errors, e => e.MemberNames.Contains("Price") && e.ErrorMessage!.Contains("required"));
        }


        [Fact]
        public void ProductBaseModel_Price_Range_NegativeValue()
        {
            var model = new ProductBaseModel
            {
                Name = "Product with negative price",
                Price = -10.50f 
            };

            var errors = ValidateModel(model);

            Assert.Single(errors);
        }

        [Fact]
        public void ProductBaseModel_Price_Range_ZeroValue_IsValid()
        {
            var model = new ProductBaseModel
            {
                Name = "Free Product",
                Price = 0.0f 
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void ProductBaseModel_Price_Range_MaxValue_IsValid()
        {
            var model = new ProductBaseModel
            {
                Name = "Very Expensive Product",
                Price = float.MaxValue 
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void ProductBaseModel_MultipleValidationErrors()
        {
            var model = new ProductBaseModel
            {
                Name = new string('X', 150), 
                Price = -5.0f 
            };

            var errors = ValidateModel(model);

            Assert.Equal(2, errors.Count);
            
        }
    }
}
