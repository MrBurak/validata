using data.validata.com.Entities;
using model.validata.com;
using System.Linq.Expressions;

namespace business.validata.com.Interfaces
{
    public interface IAbstractCommandBusiness<TEntity> where TEntity : BaseEntity, new()
    {

        Task<CommandResult<TEntity>> DeleteAsync(int id);
    }
}
