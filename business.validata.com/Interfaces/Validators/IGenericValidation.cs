using model.validata.com.Enumeration;
using model.validata.com.Validators;
using System.Linq.Expressions;

namespace business.validata.com.Interfaces.Validators
{
    public interface IGenericValidation<TEntity>
    {
        Task<ExistsResult<TEntity>?> Exists(TEntity entity, BusinessSetOperation businessSetOperation);
        Task<ExistsResult<TEntity>?> Exists(int id, BusinessSetOperation businessSetOperation);
        string? ValidateStringField(TEntity entity, string fieldName, bool isRegex, string? regex = null);
    }
}
