using System.Linq.Expressions;


namespace business.validata.com.Interfaces.Utils
{
    public interface IGenericLambdaExpressions
    {
        Expression<Func<TEntity, bool>> GetEntityByPrimaryKey<TEntity>(TEntity entity);
        Expression<Func<TEntity, bool>> GetEntityById<TEntity>(int id);
        Expression<Func<TEntity, bool>> GetEntityByUniqueValue<TEntity>(TEntity entity, string fieldName, string value, List<int> ids);
    }
}
