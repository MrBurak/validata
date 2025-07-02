using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using model.validata.com.Enumeration;
using model.validata.com.Validators;

using util.validata.com;

namespace business.validata.com.Validators
{
    public class CustomerValidation : ICustomerValidation
    {

        private readonly IGenericValidation<Customer> genericValidation;
        public CustomerValidation(IGenericValidation<Customer> genericValidation)
        {
            this.genericValidation = genericValidation;
        }


        public async Task<ValidationResult> SetAsync(Customer customer, BusinessSetOperation businessSetOperation)
        {
            var result = new ValidationResult();
            var exists = await genericValidation.Exists(customer, businessSetOperation);
            if (exists != null && exists.Entity == null) 
            {
                result.AddError((ValidationCode)exists.Code!);
            }
            

            if (StringUtil.IsEmpty(customer.FirstName))
                result.AddError(ValidationCode.FirstIsRequired);

            if (StringUtil.IsEmpty(customer.LastName))
                result.AddError(ValidationCode.LastIsRequired);

            if (StringUtil.IsEmpty(customer.Email))
                result.AddError(ValidationCode.EmailAddressInvalid);
            else if (! EmailUtil.IsValid(customer.Email))
                result.AddError(ValidationCode.EmailAddressInvalid);

            if (!StringUtil.IsEmpty(customer.Phone) && customer.Phone!.Length < 7)
                result.AddError(ValidationCode.PhoneIsInvalid);

            if (!StringUtil.IsEmpty(customer.Pobox) && customer.Pobox!.Length > 20)
                result.AddError(ValidationCode.PoboxIsInvalid);

            return result;
        }

        
    }
}
