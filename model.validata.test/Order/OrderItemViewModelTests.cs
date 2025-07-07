using model.validata.com.Order;
using System.ComponentModel.DataAnnotations;

namespace model.validata.test.Order
{
    public class OrderItemViewModelTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void OrderItemViewModel_AllPropertiesSet_TotalAmountCalculatedCorrectly()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 5,
                ProductPrice = 10.50f,
                ProductName = "Test Product"
            };

            Assert.Equal(5, model.Quantity);
            Assert.Equal(10.50f, model.ProductPrice);
            Assert.Equal("Test Product", model.ProductName);
            Assert.Equal(5 * 10.50f, model.TotalAmount); // 52.5f
        }

        [Fact]
        public void OrderItemViewModel_Quantity_CanBeSetAndGet()
        {
            var model = new OrderItemViewModel { Quantity = 10 };
            Assert.Equal(10, model.Quantity);
        }

        [Fact]
        public void OrderItemViewModel_ProductPrice_CanBeSetAndGet()
        {
            var model = new OrderItemViewModel { ProductPrice = 25.75f };
            Assert.Equal(25.75f, model.ProductPrice);
        }

        [Fact]
        public void OrderItemViewModel_ProductName_CanBeSetAndGet()
        {
            var model = new OrderItemViewModel { ProductName = "New Product" };
            Assert.Equal("New Product", model.ProductName);
        }

        [Fact]
        public void OrderItemViewModel_TotalAmount_IsZeroWhenQuantityIsZero()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 0,
                ProductPrice = 100.0f
            };
            Assert.Equal(0.0f, model.TotalAmount);
        }

        [Fact]
        public void OrderItemViewModel_TotalAmount_IsZeroWhenProductPriceIsZero()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 5,
                ProductPrice = 0.0f
            };
            Assert.Equal(0.0f, model.TotalAmount);
        }

        [Fact]
        public void OrderItemViewModel_TotalAmount_HandlesNegativeQuantityCorrectly()
        {
           var model = new OrderItemViewModel
            {
                Quantity = -2,
                ProductPrice = 15.0f
            };
            Assert.Equal(-30.0f, model.TotalAmount);
        }

        [Fact]
        public void OrderItemViewModel_TotalAmount_HandlesNegativeProductPriceCorrectly()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 3,
                ProductPrice = -5.0f
            };
            Assert.Equal(-15.0f, model.TotalAmount);
        }

        [Fact]
        public void OrderItemViewModel_TotalAmount_HandlesFloatingPointPrecision()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 3,
                ProductPrice = 0.1f
            };
           
            Assert.Equal(0.3f, model.TotalAmount, 5); 
        }

        [Fact]
        public void OrderItemViewModel_Validation_NoAttributes_NoErrors()
        {
            var model = new OrderItemViewModel
            {
                Quantity = 1,
                ProductPrice = 1.0f,
                ProductName = "Anything"
            };
            var errors = ValidateModel(model);
            Assert.Empty(errors);

            var model2 = new OrderItemViewModel(); // Default values
            errors = ValidateModel(model2);
            Assert.Empty(errors);
        }
    }
}
