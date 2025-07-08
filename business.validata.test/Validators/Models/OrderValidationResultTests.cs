using business.validata.com.Validators.Models;
using data.validata.com.Entities;
using model.validata.com.Validators;


namespace business.validata.test.Validators.Models
{
    public class OrderValidationResultTests
    {
        [Fact]
        public void Constructor_InitializesValidationResultNotNull()
        {
            var result = new OrderValidationResult();
            Assert.NotNull(result.ValidationResult);
        }

        [Fact]
        public void Constructor_InitializesProductsNotNull()
        {
            var result = new OrderValidationResult();
            Assert.NotNull(result.Products);
        }

        [Fact]
        public void Constructor_ValidationResultIsEmptyByDefault()
        {
            var result = new OrderValidationResult();

            Assert.True(result.ValidationResult.IsValid);
            Assert.Empty(result.ValidationResult.Errors);
            Assert.Null(result.ValidationResult.Entity);
        }

        [Fact]
        public void Constructor_ProductsIsEmptyByDefault()
        {
            var result = new OrderValidationResult();
            Assert.Empty(result.Products);
        }

        [Fact]
        public void ValidationResultSet_CanSetAndGet()
        {
            
            var result = new OrderValidationResult();
            var validation = new ValidationResult<Order>();
            validation.AddError("Invalid quantity");

            
            result.ValidationResult = validation;

            
            Assert.Same(validation, result.ValidationResult);
            Assert.False(result.ValidationResult.IsValid);
            Assert.Contains("Invalid quantity", result.ValidationResult.Errors);
            Assert.Null(result.ValidationResult.Entity);
        }

        [Fact]
        public void ProductsSet_CanAddAndRetrieve()
        {
            
            var result = new OrderValidationResult();
            var product1 = new Product { ProductId = 101, Name = "Laptop", Price = 12f };
            var product2 = new Product { ProductId = 102, Name = "Mouse", Price = 25f };

            
            result.Products.Add(product1);
            result.Products.Add(product2);

            
            Assert.Equal(2, result.Products.Count);
            Assert.Contains(product1, result.Products);
            Assert.Contains(product2, result.Products);
        }

        [Fact]
        public void ProductsSet_CanSetEntireList()
        {
            
            var result = new OrderValidationResult();
            var newProductList = new List<Product>
        {
            new Product { ProductId = 201, Name = "Keyboard", Price = 75f }
        };

            
            result.Products = newProductList;

            
            Assert.Single(result.Products);
            Assert.Equal("Keyboard", result.Products[0].Name);
            Assert.Same(newProductList, result.Products); 
        }
    }
}
