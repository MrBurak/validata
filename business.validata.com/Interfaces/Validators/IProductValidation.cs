using model.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;


namespace business.validata.com.Interfaces.Validators
{
    public interface IProductValidation
    {
        Task<ValidationResult<Product>> InvokeAsync(Product Product, BusinessSetOperation businessSetOperation);
    }
}
