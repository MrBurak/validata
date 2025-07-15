using business.validata.com.Interfaces.Validators;
using model.validata.com.Entities;
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


        public async Task<ValidationResult<Product>> InvokeAsync(Product product, BusinessSetOperation businessSetOperation)
        {
            
            
            var result = new ValidationResult<Product>();
            var exists = await genericValidation.Exists(product, businessSetOperation);
            if (exists != null) 
            {
                if (exists.Entity == null)
                {
                    result.AddError(exists.Code);
                    return result;
                }
                result.Entity = exists.Entity;
            }

            


            result.AddError(genericValidation.ValidateStringField(product, nameof(Product.Name), true));

            if (result.IsValid) 
            {
                var name = product.Name.Value;
                var items = await repository.GetListAsync(x => x.DeletedOn == null && x.ProductId!=product.ProductId);
                if (items.Any(x=> x.Name.Value==name)) 
                {
                    result.AddError("Product name have to be unique");
                }
            }

           
            return result;
        }        
    }
}
