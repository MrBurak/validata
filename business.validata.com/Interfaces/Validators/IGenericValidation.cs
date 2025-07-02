using model.validata.com.Enumeration;
using model.validata.com.Validators;

namespace business.validata.com.Interfaces.Validators
{
    public interface IGenericValidation<TEntity>
    {
        Task<ExistsResult<TEntity>?> Exists(TEntity entity, BusinessSetOperation businessSetOperation);
        Task<ExistsResult<TEntity>?> Exists(int id, BusinessSetOperation businessSetOperation);
        Task<string?> ValidateStringField(TEntity entity, string fieldName, bool isRegex, bool isUnique, List<int>? id = null, string? regex = null);
    }
}
