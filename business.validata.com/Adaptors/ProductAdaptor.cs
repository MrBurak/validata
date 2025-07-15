using business.validata.com.Interfaces.Adaptors;
using model.validata.com.Entities;
using model.validata.com.Product;
using model.validata.com.ValueObjects.Product;

namespace business.validata.com.Adaptors
{
    public class ProductAdaptor : IProductAdaptor
    {
        public Product Invoke(ProductModel model) 
        {
            return new Product(model.ProductId, new ProductName(model.Name!), new ProductPrice(model.Price));
        }

        public ProductModel Invoke(Product model)
        {
            return new ProductModel
            {
                ProductId = model.ProductId,
                Name = model.Name.Value,
                Price = model.PriceValue
            };
        }

        public IEnumerable<ProductModel> Invoke(IEnumerable<Product> products)
        {
            return products.Select(product => new ProductModel
            {
                ProductId = product.ProductId,
                Name = product.Name.Value,
                Price = product.PriceValue
            });
        }
    }
}
