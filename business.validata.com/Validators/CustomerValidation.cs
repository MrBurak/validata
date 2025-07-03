using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;



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
            
            if (businessSetOperation == BusinessSetOperation.Create) 
            {
                customer.CustomerId = 0;
            }
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
            
            
            result.AddError(await genericValidation.ValidateStringField(customer, nameof(Customer.FirstName), true, false));
            result.AddError(await genericValidation.ValidateStringField(customer, nameof(Customer.LastName), true, false));
            if (businessSetOperation.Equals(BusinessSetOperation.Create)) 
            {
                var ids = (await repository.GetListAsync(x => x.DeletedOn == null)).Select(x=> x.CustomerId).ToList();

                result.AddError(await genericValidation.ValidateStringField(customer, nameof(Customer.Email), true, true, ids ,@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"));
            }
            
            result.AddError(await genericValidation.ValidateStringField(customer, nameof(Customer.Pobox), true, false));
            return result;
        }        
    }
}
