using model.validata.com.Customer;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Customer
{
    public class CustomerBaseModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CustomerBaseModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void CustomerBaseModel_FirstName_Required()
        {
            var model = new CustomerBaseModel
            {
                FirstName = null, 
                LastName = "Doe",
                Address = "123 Main St",
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors); 
        }

        [Fact]
        public void CustomerBaseModel_FirstName_MaxLength()
        {
            var model = new CustomerBaseModel
            {
                FirstName = new string('A', 129), 
                LastName = "Doe",
                Address = "123 Main St",
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_LastName_Required()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = null, 
                Address = "123 Main St",
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("LastName") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_LastName_MaxLength()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = new string('B', 129), 
                Address = "123 Main St",
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("LastName") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_Address_Required()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = null, 
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Address") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_Address_MaxLength()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = new string('C', 513),
                Pobox = "12345"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Address") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_Pobox_Required()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                Pobox = null 
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_Pobox_MaxLength()
        {
            var model = new CustomerBaseModel
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                Pobox = "12345678901" 
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerBaseModel_MultipleValidationErrors()
        {
            var model = new CustomerBaseModel
            {
                FirstName = null, 
                LastName = new string('B', 150), 
                Address = null, 
                Pobox = "1234567890123" 
            };

            var errors = ValidateModel(model);

            Assert.Equal(4, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("LastName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Address"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox"));
        }
    }
}
