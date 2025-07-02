

using model.validata.com.Validators;

namespace business.validata.com.Interfaces.Utils
{
    public interface IStringFieldValidation<TEntity>
    {
        Task<string?> InvokeAsnc(StringField<TEntity> stringField);
    }
}
