using model.validata.com;


namespace model.validata.test
{
    public class CommandResultTests
    {
        [Fact]
        public void Constructor_InitializesValidationsAsEmptyList()
        {
            var commandResult = new CommandResult<string>();

            Assert.NotNull(commandResult.Validations);
            Assert.Empty(commandResult.Validations);
        }

        [Fact]
        public void CanAddAndRetrieveValidations()
        {
            var commandResult = new CommandResult<int>();
            var validationMessage1 = "Field 'Name' is required.";
            var validationMessage2 = "Invalid email format.";

            commandResult.Validations.Add(validationMessage1);
            commandResult.Validations.Add(validationMessage2);

            Assert.Contains(validationMessage1, commandResult.Validations);
            Assert.Contains(validationMessage2, commandResult.Validations);
            Assert.Equal(2, commandResult.Validations.Count);
        }

        [Fact]
        public void InheritsPropertiesFromQueryResult()
        {
            var commandResult = new CommandResult<bool>
            {
                Result = true,
                Success = true,
                Exception = "No exception"
            };

            Assert.True(commandResult.Result);
            Assert.True(commandResult.Success);
            Assert.Equal("No exception", commandResult.Exception);
        }

        [Fact]
        public void CanCombineInheritedAndNewProperties()
        {
            var product = new Product { ProductId = 1, Name = "Test Product" };
            var commandResult = new CommandResult<Product>
            {
                Result = product,
                Success = false,
                Exception = "Validation failed.",
                Validations = { "Name must be unique.", "Price cannot be zero." }
            };

            Assert.NotNull(commandResult.Result);
            Assert.Equal(1, commandResult.Result!.ProductId);
            Assert.False(commandResult.Success);
            Assert.Equal("Validation failed.", commandResult.Exception);
            Assert.Equal(2, commandResult.Validations.Count);
            Assert.Contains("Name must be unique.", commandResult.Validations);
            Assert.Contains("Price cannot be zero.", commandResult.Validations);
        }

        [Fact]
        public void ValidationsListIsEmptyByDefault()
        {
            var commandResult = new CommandResult<object>();
            Assert.Empty(commandResult.Validations);
        }

        public class Product
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
