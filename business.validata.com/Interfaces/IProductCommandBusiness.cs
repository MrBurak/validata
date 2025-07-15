using model.validata.com.Entities;
using model.validata.com;
using model.validata.com.Enumeration;
using model.validata.com.Product;
namespace business.validata.com.Interfaces
{
    public interface IProductCommandBusiness
    {
        Task<CommandResult<ProductModel>> InvokeAsync(Product Product, BusinessSetOperation businessSetOperation);
        Task<CommandResult<Product>> DeleteAsync(int id);
    }
}
