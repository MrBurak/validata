using data.validata.com.Entities;
using model.validata.com;
using System.Linq.Expressions;

namespace business.validata.com.Interfaces
{
    public interface IAbstractCommandBusiness<TEntity> where TEntity : BaseEntity, new()
    {
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? expression = null);
        Task<TEntity?> GetEntityAsync(int id, Expression<Func<TEntity, bool>>? expression = null);
        Task<CommandResult<TEntity>> DeleteAsync(int id);
    }
}
