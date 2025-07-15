using model.validata.com.ValueObjects.Base;
using System.Reflection;


namespace model.validata.com.ValueObjects.Util
{
    public class ValueObjectUtil
    {
        

        public static string GetValue<TEntity>(TEntity obj, string fieldName)
        {
            if (obj == null) return "";

            PropertyInfo? property = obj.GetType().GetProperty(fieldName);
            if (property == null) return "";

            var val = property.GetValue(obj);
            if (val == null || val is not ValueObject valueObj) return "";

            PropertyInfo? propertyValue = valueObj.GetType().GetProperty("Value");
            if (propertyValue == null) return "";

            var valueVal = propertyValue.GetValue(valueObj);
            return valueVal as string ?? "";
        }
    }
}

