using data.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;


namespace business.validata.com.Interfaces.Validators
{
    public interface ICustomerValidation
    {
        Task<ValidationResult> SetAsync(Customer customer, BusinessSetOperation businessSetOperation);
    }
}
