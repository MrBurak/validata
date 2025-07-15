using model.validata.com.ValueObjects.Product;
namespace model.validata.test.Entities
{


    public class ProductTests
    {
        private ProductName ValidName() => new ProductName("Valid Product");
        private ProductPrice ValidPrice() => new ProductPrice(99.99m);

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var name = ValidName();
            var price = ValidPrice();
            var product = new com.Entities.Product(1, name, price);

            Assert.Equal(1, product.ProductId);
            Assert.Equal(name.Value, product.NameValue);
            Assert.Equal(price.Value, product.PriceValue);
            Assert.Equal(name.Value, product.Name.Value);
            Assert.Equal(price.Value, product.Price.Value);
            Assert.NotEqual(default, product.CreatedOnTimeStamp);
            Assert.NotEqual(default, product.LastModifiedTimeStamp);
        }

        [Fact]
        public void Constructor_ShouldThrowForNullName()
        {
            var price = ValidPrice();
            Assert.Throws<ArgumentNullException>(() => new com.Entities.Product(1, null!, price));
        }

        [Fact]
        public void Constructor_ShouldThrowForNullPrice()
        {
            var name = ValidName();
            Assert.Throws<ArgumentNullException>(() => new com.Entities.Product(1, name, null!));
        }

        [Fact]
        public void ChangeName_ShouldUpdateNameAndTimestamp()
        {
            var product = new com.Entities.Product(1, ValidName(), ValidPrice());
            var newName = new ProductName("New Product Name");
            var oldTimestamp = product.LastModifiedTimeStamp;

            product.ChangeName(newName);

            Assert.Equal(newName.Value, product.NameValue);
            Assert.Equal(newName.Value, product.Name.Value);
            Assert.True(product.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void ChangeName_ShouldThrowOnNull()
        {
            var product = new com.Entities.Product(1, ValidName(), ValidPrice());
            Assert.Throws<ArgumentNullException>(() => product.ChangeName(null!));
        }

        [Fact]
        public void UpdatePrice_ShouldUpdatePriceAndTimestamp()
        {
            var product = new com.Entities.Product(1, ValidName(), ValidPrice());
            var newPrice = new ProductPrice(150.00m);
            var oldTimestamp = product.LastModifiedTimeStamp;

            product.UpdatePrice(newPrice);

            Assert.Equal(newPrice.Value, product.PriceValue);
            Assert.Equal(newPrice.Value, product.Price.Value);
            Assert.True(product.LastModifiedTimeStamp > oldTimestamp);
        }

        [Fact]
        public void UpdatePrice_ShouldThrowOnNull()
        {
            var product = new com.Entities.Product(1, ValidName(), ValidPrice());
            Assert.Throws<ArgumentNullException>(() => product.UpdatePrice(null!));
        }

        [Fact]
        public void LoadValueObjectsFromBackingFields_ShouldInitializeValueObjects()
        {
            var product = new com.Entities.Product(1, ValidName(), ValidPrice());

            
            product.GetType().GetProperty("Name")!.SetValue(product, null);
            product.GetType().GetProperty("Price")!.SetValue(product, null);

            product.LoadValueObjectsFromBackingFields();

            Assert.NotNull(product.Name);
            Assert.Equal(product.NameValue, product.Name.Value);

            Assert.NotNull(product.Price);
            Assert.Equal(product.PriceValue, product.Price.Value);
        }
    }

}
