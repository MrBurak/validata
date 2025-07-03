using System.Linq.Expressions;


namespace data.validata.com.Interfaces.Repository
{
    public interface ICommandRepository<T>
        where T : class
    {
      
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> query);
        Task<T?> GetEntityAsync(Expression<Func<T, bool>> query);
        Task UpdateAsync(Expression<Func<T, bool>> filterExpression, List<Action<T>> properties);
        Task<T> AddAsync(T entity);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> query);

    }
}