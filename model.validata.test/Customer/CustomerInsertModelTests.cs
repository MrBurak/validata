using model.validata.com.Customer;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Customer
{
    public class CustomerInsertModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CustomerInsertModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new CustomerInsertModel
            {
                FirstName = "Jane",
                LastName = "Doe",
                Address = "456 Oak Ave",
                Pobox = "67890",
                Email = "jane.doe@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void CustomerInsertModel_Email_Required()
        {
            var model = new CustomerInsertModel
            {
                FirstName = "Jane",
                LastName = "Doe",
                Address = "456 Oak Ave",
                Pobox = "67890",
                Email = null 
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Email") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerInsertModel_Email_MaxLength()
        {
            var model = new CustomerInsertModel
            {
                FirstName = "Jane",
                LastName = "Doe",
                Address = "456 Oak Ave",
                Pobox = "67890",
                Email = new string('a', 129) + "@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Email") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerInsertModel_InheritedProperties_RequiredAndMaxLengthChecks()
        {
            var model = new CustomerInsertModel
            {
                FirstName = null, 
                LastName = new string('X', 150), 
                Address = "Some address",
                Pobox = "12345",
                Email = "test@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Equal(2, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName") && e.ErrorMessage!.Contains("required"));
            Assert.Contains(errors, e => e.MemberNames.Contains("LastName") && e.ErrorMessage!.Contains("length"));
        }

        [Fact]
        public void CustomerInsertModel_MultipleValidationErrors_AcrossBaseAndDerived()
        {
            var model = new CustomerInsertModel
            {
                FirstName = null, 
                LastName = "Doe",
                Address = null,
                Pobox = "12345678901", 
                Email = new string('b', 129) + "@domain.com" 
            };

            var errors = ValidateModel(model);

            Assert.Equal(4, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Address"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Email"));
        }
    }
}
