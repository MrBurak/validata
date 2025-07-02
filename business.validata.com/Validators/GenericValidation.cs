using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Interfaces.Repository;
using model.validata.com.Enumeration;
using model.validata.com.Validators;


namespace business.validata.com.Validators
{
    public class GenericValidation<TEntity> : IGenericValidation<TEntity> where TEntity : class
    {
        private readonly IDataRepository<TEntity> repository;
        private readonly IGenericLambdaExpressions lambdaExpressions;
        public GenericValidation
        (
            IDataRepository<TEntity> repository, 
            IGenericLambdaExpressions lambdaExpressions, 
            BusinessSetOperation businessSetOperation
        )
        {
            this.repository = repository;
            this.lambdaExpressions = lambdaExpressions;
        }
        public async Task<ExistsResult<TEntity>?> Exists(TEntity entity, BusinessSetOperation businessSetOperation)
        {
            if(businessSetOperation!=BusinessSetOperation.Update && businessSetOperation != BusinessSetOperation.Delete) return null;
            var exists = await this.repository.GetEntityAsync(lambdaExpressions.GetEntityByPrimaryKey(entity));
            return new ExistsResult<TEntity> { Entity = exists };
        }
    }
}
