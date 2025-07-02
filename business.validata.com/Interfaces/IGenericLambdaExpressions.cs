using System.Linq.Expressions;


namespace business.validata.com.Interfaces
{
    public interface IGenericLambdaExpressions
    {
        Expression<Func<TEntity, bool>> GetEntityByPrimaryKey<TEntity>(TEntity entity);
    }
}
