using business.validata.com.Interfaces.Validators;
using model.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;
using System.Linq.Expressions;



namespace business.validata.com.Validators
{
    public class CustomerValidation : ICustomerValidation
    {

        private readonly IGenericValidation<Customer> genericValidation;
        private readonly ICommandRepository<Customer> repository;
        public CustomerValidation(IGenericValidation<Customer> genericValidation, ICommandRepository<Customer> repository)
        {
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(repository);
            this.genericValidation = genericValidation;
            this.repository = repository;
        }


        public async Task<ValidationResult<Customer>> InvokeAsync(Customer customer, BusinessSetOperation businessSetOperation)
        {
            
           
            var result = new ValidationResult<Customer>();
            var exists = await genericValidation.Exists(customer, businessSetOperation);
            if (exists != null) 
            {
                if (exists.Entity == null)
                {
                    result.AddError(exists.Code);
                    return result;
                }
                result.Entity = exists.Entity;
            }
            
            
            result.AddError(genericValidation.ValidateStringField(customer, nameof(Customer.FirstName), true, null));
            result.AddError(genericValidation.ValidateStringField(customer, nameof(Customer.LastName), true, null));
            if (businessSetOperation.Equals(BusinessSetOperation.Create)) 
            {
                

                Expression<Func<Customer, bool>> expression= x=> x.Email.Value.Equals(customer.EmailValue);
                result.AddError(genericValidation.ValidateStringField(customer, nameof(Customer.Email), true, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"));
                if (result.IsValid)
                {
                    var email = customer.Email.Value;
                    var items = await repository.GetListAsync(x => x.DeletedOn == null && x.CustomerId != customer.CustomerId);
                    if (items.Any(x => x.Email.Value == email))
                    {
                        result.AddError("Customer email have to be unique");
                    }
                }
            }
            
            result.AddError(genericValidation.ValidateStringField(customer, nameof(Customer.Pobox), true, null));
            return result;
        }        
    }
}
