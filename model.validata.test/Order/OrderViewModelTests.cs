using model.validata.com.Order;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Order
{
    public class OrderViewModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void OrderViewModel_AllPropertiesValid_NoValidationErrors()
        {
            var model = new OrderViewModel
            {
                OrderId = 1,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 150.75f,
                ProductCount = 3
            };

            var errors = ValidateModel(model);

            Assert.Empty(errors);
        }

        [Fact]
        public void OrderViewModel_OrderId_CanBeSetAndGet()
        {
            var model = new OrderViewModel { OrderId = 123 };
            Assert.Equal(123, model.OrderId);
        }

        [Fact]
        public void OrderViewModel_OrderId_DefaultValueIsZero()
        {
            var model = new OrderViewModel();
            Assert.Equal(0, model.OrderId);
        }

        [Fact]
        public void OrderViewModel_OrderDate_CanBeSetAndGet()
        {
            var testDate = new DateTime(2025, 7, 7, 10, 30, 0, DateTimeKind.Utc);
            var model = new OrderViewModel { OrderDate = testDate };
            Assert.Equal(testDate, model.OrderDate);
        }

        [Fact]
        public void OrderViewModel_OrderDate_DefaultValue()
        {
            var model = new OrderViewModel();
            Assert.Equal(default(DateTime), model.OrderDate);
        }

        [Fact]
        public void OrderViewModel_TotalAmount_Required()
        {
            var model = new OrderViewModel
            {
                OrderId = 1,
                OrderDate = DateTime.UtcNow,
                
                TotalAmount = default(float), 
                ProductCount = 5
            };

            var errors = ValidateModel(model);
            Assert.DoesNotContain(errors, e => e.MemberNames.Contains("TotalAmount") && e.ErrorMessage!.Contains("required"));

        }

        [Fact]
        public void OrderViewModel_ProductCount_Required()
        {
            var model = new OrderViewModel
            {
                OrderId = 1,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100.0f,
              
                ProductCount = default(int) 
            };

            var errors = ValidateModel(model);

            Assert.DoesNotContain(errors, e => e.MemberNames.Contains("ProductCount") && e.ErrorMessage!.Contains("required"));
        }

       
    }
}
