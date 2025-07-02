using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using business.validata.com.Validators;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Validators;
using util.validata.com;

namespace business.validata.com.Utils
{
    public class StringFieldValidation<TEntity> : IStringFieldValidation<TEntity> where TEntity : class
    {
        

        private readonly IDataRepository<TEntity> repository;
        
        private readonly IGenericLambdaExpressions genericLambdaExpressions;
        
       

        public StringFieldValidation(
            IDataRepository<TEntity> repository, 
            IGenericLambdaExpressions genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            this.repository = repository;
            this.genericLambdaExpressions = genericLambdaExpressions;
        }

        
        public async Task<string?> InvokeAsnc(StringField<TEntity> stringField)
        {
            string value = ObjectUtil.GetValue(stringField.Entity, stringField.Field!);

            if (StringUtil.IsEmpty(value)) return stringField.EmptyMesssage;
            if (stringField.CheckRegex) 
            {
                if (!StringUtil.IsAlphaNumeric(value, stringField.Regex)) return stringField.RegexMesssage;
            }
            if (stringField.CheckUnique) 
            {
                
                var expression = genericLambdaExpressions.GetEntityByUniqueValue(stringField.Entity, stringField.Field!, value, stringField.Ids);
                var exists = await repository.GetEntityAsync(expression!);
                if (exists!=null) { return stringField.UnixMesssage; }
            }
            return null;
        }



        
    }
}