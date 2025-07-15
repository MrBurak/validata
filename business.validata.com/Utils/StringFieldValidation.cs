using business.validata.com.Interfaces.Utils;
using model.validata.com.Validators;
using model.validata.com.ValueObjects.Util;
using util.validata.com;

namespace business.validata.com.Utils
{
    public class StringFieldValidation<TEntity> : IStringFieldValidation<TEntity> where TEntity : class
    {
        

        
        public string? Invoke(StringField<TEntity> stringField)
        {

            string value = ValueObjectUtil.GetValue(stringField.Entity, stringField.Field!)!;

            if (StringUtil.IsEmpty(value)) return stringField.EmptyMesssage;
            if (stringField.CheckRegex) 
            {
                if (!StringUtil.IsAlphaNumeric(value, stringField.Regex)) return stringField.RegexMesssage;
            }
            
            return null;
        }



        
    }
}