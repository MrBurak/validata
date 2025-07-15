

using model.validata.com.Validators;

namespace business.validata.com.Interfaces.Utils
{
    public interface IStringFieldValidation<TEntity>
    {
        string? Invoke(StringField<TEntity> stringField);
    }
}
