using Newtonsoft.Json;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace util.validata.com
{
    public class ObjectUtil
    {
      
        public static string GetValue<T>(T obj, string fieldName)
        {
            if (obj == null) return "";
            PropertyInfo? property = obj.GetType().GetProperty(fieldName);
            if (property == null) return "";
            var val = property.GetValue(obj);
            if (val == null) return string.Empty;
            return (string)val;
        }

        public static Expression<Func<T, bool>> ConcatLambdaExpression<T>(Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            var invokedThird = Expression.Invoke(secondExpression, firstExpression.Parameters.Cast<Expression>());
            var finalExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(firstExpression.Body, invokedThird), firstExpression.Parameters);
            return finalExpression;
        }

        public static Expression<Func<T, bool>> NotContainsLambdaExpression<T>(List<int> ids, string keyPropName)
        {
            var eParam = Expression.Parameter(typeof(T), "e");
            var method = ids.GetType().GetMethod("Contains");
            var call = Expression.Call(Expression.Constant(ids), method!, Expression.Property(eParam, keyPropName));
            return Expression.Lambda<Func<T, bool>>(Expression.Not(call), eParam);
        }

        public static T ConvertObj<T,D>(D obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}
