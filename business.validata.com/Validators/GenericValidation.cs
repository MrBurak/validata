using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;


namespace business.validata.com.Validators
{
    public class GenericValidation<TEntity> : IGenericValidation<TEntity> where TEntity : class
    {
        private readonly ICommandRepository<TEntity> repository;
        private readonly IGenericLambdaExpressions lambdaExpressions;
        private readonly IStringFieldValidation<TEntity> stringFieldValidation;
        public GenericValidation
        (
            ICommandRepository<TEntity> repository, 
            IGenericLambdaExpressions lambdaExpressions,
            IStringFieldValidation<TEntity> stringFieldValidation
        )
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(lambdaExpressions);
            ArgumentNullException.ThrowIfNull(stringFieldValidation);
            this.repository = repository;
            this.lambdaExpressions = lambdaExpressions;
            this.stringFieldValidation = stringFieldValidation;
        }
        public async Task<ExistsResult<TEntity>?> Exists(TEntity entity, BusinessSetOperation businessSetOperation)
        {
            if(businessSetOperation!=BusinessSetOperation.Update && businessSetOperation != BusinessSetOperation.Delete && businessSetOperation != BusinessSetOperation.Get) return null;
            var exists = await this.repository.GetEntityAsync(lambdaExpressions.GetEntityByPrimaryKey(entity));
            return new ExistsResult<TEntity> { Entity = exists };
        }

        public async Task<ExistsResult<TEntity>?> Exists(int id, BusinessSetOperation businessSetOperation)
        {
            if (businessSetOperation != BusinessSetOperation.Update && businessSetOperation != BusinessSetOperation.Delete && businessSetOperation != BusinessSetOperation.Get) return null;
            var exists = await this.repository.GetEntityAsync(lambdaExpressions.GetEntityById<TEntity>(id));
            return new ExistsResult<TEntity> { Entity = exists };
        }

        public string? ValidateStringField(TEntity entity, string fieldName, bool isRegex, string? regex = null)
        {
            var stringfield = new StringField<TEntity>
            {
                Entity = entity,
                CheckRegex = isRegex,
                Field = fieldName, 
                Regex = regex,
            };
            return stringFieldValidation.Invoke(stringfield);
           
        }
    }
}
