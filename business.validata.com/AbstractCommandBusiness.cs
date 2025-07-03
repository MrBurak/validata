using business.validata.com.Interfaces;
using business.validata.com.Interfaces.Utils;
using business.validata.com.Interfaces.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.Enumeration;
using System.Linq.Expressions;
using util.validata.com;

namespace business.validata.com
{
    public abstract class AbstractCommandBusiness<TEntity> : IAbstractCommandBusiness<TEntity> where TEntity : BaseEntity, new()
    {   
        private readonly IGenericLambdaExpressions genericLambdaExpressions;
        private readonly IGenericValidation<TEntity> genericValidation;
        private readonly ICommandRepository<TEntity> repository;
        public AbstractCommandBusiness(IGenericValidation<TEntity> genericValidation, ICommandRepository<TEntity> repository, IGenericLambdaExpressions genericLambdaExpressions)
        {
            ArgumentNullException.ThrowIfNull(genericValidation);
            ArgumentNullException.ThrowIfNull(repository); 
            ArgumentNullException.ThrowIfNull(genericLambdaExpressions);
            this.genericValidation = genericValidation;
            this.repository = repository;
            this.genericLambdaExpressions = genericLambdaExpressions;
        }
      
        public virtual async Task<CommandResult<TEntity>> DeleteAsync(int id) 
        {
            CommandResult<TEntity> apiResult = new CommandResult<TEntity>();
            var exist = await genericValidation.Exists(id, BusinessSetOperation.Delete);
            if (exist!=null && exist.Code!=null) 
            {
                apiResult.Validations.Add(exist.Code);
                
            }
            List<Action<TEntity>> properties = new()
            {
                x=> {
                    x.DeletedOn = DateTimeUtil.SystemTime;
                    x.LastModifiedTimeStamp = DateTimeUtil.SystemTime;
                    x.OperationSourceId = (int) BusinessOperationSource.Api;
                }
            };
            try
            {
                await repository.UpdateAsync(genericLambdaExpressions.GetEntityById<TEntity>(id), properties);
                apiResult.Success = true;
            }
            catch (Exception ex) 
            {
                apiResult.Exception = ex.Message;
                apiResult.Success=false;
            }
            return apiResult;
              
        }

        public async Task<TEntity?> BaseInvokeAsync(TEntity entity, TEntity request, BusinessSetOperation businessSetOperation, List<Action<TEntity>> properties)
        {
            
            try
            {
                if (businessSetOperation == BusinessSetOperation.Create)
                {
                    return await repository.AddAsync(request);
                }
                else
                {
                    var query= genericLambdaExpressions.GetEntityByPrimaryKey(entity);
                    await repository.UpdateAsync(query, properties); ;
                    return await repository.GetEntityAsync(query);
                }

            }
            catch
            {
                throw;
            }

        }
    }
}
