using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;



namespace business.validata.com.Validators
{
    public class ProductValidation : IProductValidation
    {

        private readonly IGenericValidation<Product> genericValidation;
        private readonly ICommandRepository<Product> repository;
        public ProductValidation(IGenericValidation<Product> genericValidation, ICommandRepository<Product> repository)
        {
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(repository);
            this.genericValidation = genericValidation;
            this.repository = repository;
        }


        public async Task<ValidationResult<Product>> InvokeAsync(Product Product, BusinessSetOperation businessSetOperation)
        {
            
            if (businessSetOperation == BusinessSetOperation.Create) 
            {
                Product.ProductId = 0;
            }
            var result = new ValidationResult<Product>();
            var exists = await genericValidation.Exists(Product, businessSetOperation);
            if (exists != null) 
            {
                if (exists.Entity == null)
                {
                    result.AddError(exists.Code);
                    return result;
                }
                result.Entity = exists.Entity;
            }

            var ids = (await repository.GetListAsync(x => x.DeletedOn == null)).Select(x => x.ProductId).ToList();
            result.AddError(await genericValidation.ValidateStringField(Product, nameof(Product.Name), true, true, ids));
           
            return result;
        }        
    }
}
