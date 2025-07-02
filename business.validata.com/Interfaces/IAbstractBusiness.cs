using data.validata.com.Entities;
using model.validata.com;
using System.Linq.Expressions;

namespace business.validata.com.Interfaces
{
    public interface IAbstractBusiness<TEntity> where TEntity : BaseEntity, new()
    {
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? expression = null);
        Task<TEntity?> GetEntityAsync(int id, Expression<Func<TEntity, bool>>? expression = null);
        Task<ApiResult<TEntity>> DeleteAsync(int id);
    }
}
