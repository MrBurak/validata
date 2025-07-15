using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.validata.com.Interfaces.Adaptors
{
    public interface IProductAdaptor
    {
        Product Invoke(ProductModel model);
        ProductModel Invoke(Product model);
        IEnumerable<ProductModel> Invoke(IEnumerable<Product> products);
    }
}
