using business.validata.com.Validators.Models;
using data.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;


namespace business.validata.com.Interfaces.Validators
{
    public interface IOrderValidation
    {
        Task<OrderValidationResult> InvokeAsync(Order order, BusinessSetOperation businessSetOperation);
    }
}
