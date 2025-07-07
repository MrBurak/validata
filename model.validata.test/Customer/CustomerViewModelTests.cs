using model.validata.com.Customer;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Customer
{
    public class CustomerViewModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CustomerViewModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Address = "789 Pine Rd",
                Pobox = "11223",
                Email = "alice.smith@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void CustomerViewModel_CustomerId_CanBeSetAndGet()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 99
            };

            Assert.Equal(99, model.CustomerId);
        }

        [Fact]
        public void CustomerViewModel_CustomerId_DefaultValueIsZero()
        {
            var model = new CustomerViewModel();

            Assert.Equal(0, model.CustomerId);
        }

        [Fact]
        public void CustomerViewModel_Email_Required()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Address = "789 Pine Rd",
                Pobox = "11223",
                Email = null 
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Email") && e.ErrorMessage!.Contains("required"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerViewModel_Email_MaxLength()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Address = "789 Pine Rd",
                Pobox = "11223",
                Email = new string('a', 129) + "@test.com"
            };

            var errors = ValidateModel(model);

            Assert.Contains(errors, e => e.MemberNames.Contains("Email") && e.ErrorMessage!.Contains("length"));
            Assert.Single(errors);
        }

        [Fact]
        public void CustomerViewModel_InheritedProperties_RequiredChecks()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = null,
                LastName = "Smith",
                Address = null, 
                Pobox = "11223",
                Email = "test@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Equal(2, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName") && e.ErrorMessage!.Contains("required"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Address") && e.ErrorMessage!.Contains("required"));
        }

        [Fact]
        public void CustomerViewModel_InheritedProperties_MaxLengthChecks()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = "Alice",
                LastName = new string('S', 150), 
                Address = "789 Pine Rd",
                Pobox = "11223445566", 
                Email = "test@example.com"
            };

            var errors = ValidateModel(model);

            Assert.Equal(2, errors.Count);
            Assert.Contains(errors, e => e.MemberNames.Contains("LastName") && e.ErrorMessage!.Contains("length"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox") && e.ErrorMessage!.Contains("length"));
        }

        [Fact]
        public void CustomerViewModel_MultipleValidationErrors_AcrossBaseAndDerived()
        {
            var model = new CustomerViewModel
            {
                CustomerId = 1,
                FirstName = null,
                LastName = new string('L', 150),
                Address = null,
                Pobox = "1234567890123", 
                Email = new string('e', 129) + "@domain.com" 
            };

            var errors = ValidateModel(model);

            Assert.Equal(5, errors.Count); 
            Assert.Contains(errors, e => e.MemberNames.Contains("FirstName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("LastName"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Address"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Pobox"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Email"));
        }
    }
}
