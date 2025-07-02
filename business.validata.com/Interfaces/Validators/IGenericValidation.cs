using model.validata.com.Enumeration;
using model.validata.com.Validators;

namespace business.validata.com.Interfaces.Validators
{
    public interface IGenericValidation<TEntity>
    {
        Task<ExistsResult<TEntity>?> Exists(TEntity entity, BusinessSetOperation businessSetOperation);
    }
}
