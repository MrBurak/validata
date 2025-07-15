using business.validata.com.Adaptors;
using FluentAssertions;
using model.validata.com.Entities;
using model.validata.com.Product;
using model.validata.com.ValueObjects.Product;
namespace business.validata.test.Adaptors
{
 

    public class ProductAdaptorTests
    {
        private readonly ProductAdaptor sut;

        public ProductAdaptorTests()
        {
            sut = new ProductAdaptor();
        }

        [Fact]
        public void Invoke_WithProductModel_ShouldReturnProduct()
        {
            
            var model = new ProductModel
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 99.99m
            };

            
            var product = sut.Invoke(model);

            
            product.ProductId.Should().Be(model.ProductId);
            product.Name.Value.Should().Be(model.Name);
            product.PriceValue.Should().Be(model.Price);
        }

        [Fact]
        public void Invoke_WithProduct_ShouldReturnProductModel()
        {
            
            var product = new Product(
                productId: 2,
                name: new ProductName("Product X"),
                price: new ProductPrice(45.5m)
            );

            
            var model = sut.Invoke(product);

            
            model.ProductId.Should().Be(2);
            model.Name.Should().Be("Product X");
            model.Price.Should().Be(45.5m);
        }

        [Fact]
        public void Invoke_WithProductList_ShouldReturnProductModelList()
        {
            
            var products = new List<Product>
        {
            new Product(1, new ProductName("Item A"), new ProductPrice(10)),
            new Product(2, new ProductName("Item B"), new ProductPrice(20))
        };

            
            var result = sut.Invoke(products).ToList();

            
            result.Should().HaveCount(2);
            result[0].ProductId.Should().Be(1);
            result[0].Name.Should().Be("Item A");
            result[0].Price.Should().Be(10);
        }
    }

}
