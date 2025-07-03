using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace util.validata.com
{
    [ExcludeFromCodeCoverage]
    public class ObjectUtil
    {
        

        public static void SetValue<T>(T obj, string fieldName, object? value)
        {
            if (obj == null) return;
            PropertyInfo? property = obj.GetType().GetProperty(fieldName);
            if (property == null) return;
            property.SetValue(obj, value);
        }
        public static string GetValue<T>(T obj, string fieldName)
        {
            if (obj == null) return "";
            PropertyInfo? property = obj.GetType().GetProperty(fieldName);
            if (property == null) return "";
            var val = property.GetValue(obj);
            if (val == null) return string.Empty;
            return (string)val;
        }

        public static dynamic? GetObjectValue<T>(T obj, string fieldName)
        {
            if (obj == null) return "";
            PropertyInfo? property = obj.GetType().GetProperty(fieldName);
            if (property == null) return "";
            var val = property.GetValue(obj);
            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            return (val == null) ? null : Convert.ChangeType(val, t);
            
        }

        public static IEnumerable<object?> GetObjectPropValues<T>(PropertyInfo property, IEnumerable<T> objects)
        {
            return objects.Select(x => property.GetValue(x));
        }

        public static PropertyInfo FindKeyProperty<T>()
        {
            Type entityType = typeof(T);
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(KeyAttribute), true).Any())
                {
                    return property;
                }
            }

            throw new InvalidOperationException("No property with KeyAttribute found.");
        }

        

        public static Expression<Func<T, bool>> ConcatLambdaExpression<T>(Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            var invokedThird = Expression.Invoke(secondExpression, firstExpression.Parameters.Cast<Expression>());
            var finalExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(firstExpression.Body, invokedThird), firstExpression.Parameters);
            return finalExpression;
        }

        public static Expression<Func<T, bool>> ContainsLambdaExpression<T>(List<int> ids, string keyPropName) 
        {
            var eParam = Expression.Parameter(typeof(T), "e");
            var method = ids.GetType().GetMethod("Contains");
            var call = Expression.Call(Expression.Constant(ids), method!, Expression.Property(eParam, keyPropName));
            return Expression.Lambda<Func<T, bool>>(call, eParam);
        }

        public static Expression<Func<T, bool>> NotContainsLambdaExpression<T>(List<int> ids, string keyPropName)
        {
            var eParam = Expression.Parameter(typeof(T), "e");
            var method = ids.GetType().GetMethod("Contains");
            var call = Expression.Call(Expression.Constant(ids), method!, Expression.Property(eParam, keyPropName));
            return Expression.Lambda<Func<T, bool>>(Expression.Not(call), eParam);
        }



        public static Expression<Func<T, bool>> EqualLambdaExpression<T>(int id, string keyPropName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var left = Expression.Property(parameter, keyPropName);
            Expression<Func<int>> right = () => id;
            var convertedRight = Expression.Convert(right.Body, id.GetType());
            var body = Expression.Equal(left, convertedRight);
            return Expression.Lambda<Func<T, bool>>(body, new ParameterExpression[] { parameter });

        }

        public static T GetEnumMember<T>(string enumMember)
            where T : Enum
        {
            var type = typeof(T);

            return type.GetEnumValues().Cast<T>()
                .Single(x => x.ToString() == enumMember);
        }
        public static T Copy<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json)!;
        }

        public static T ConvertObj<T,D>(D obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json)!;
        }


    }
}
