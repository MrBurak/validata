using data.validata.com.Entities;
using model.validata.com;

namespace business.validata.com.Interfaces
{
    public interface IAbstractCommandBusiness<TEntity> where TEntity : BaseEntity, new()
    {

        Task<CommandResult<TEntity>> DeleteAsync(int id);
    }
}
