using System.Linq.Expressions;


namespace business.validata.com.Interfaces.Utils
{
    public interface IGenericLambdaExpressions
    {
        Expression<Func<TEntity, bool>> GetEntityByPrimaryKey<TEntity>(TEntity entity);
        Expression<Func<TEntity, bool>> GetEntityById<TEntity>(int id);
    }
}
