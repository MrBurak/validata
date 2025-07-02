using business.validata.com.Interfaces.Utils;
using business.validata.com.Validators;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using util.validata.com;


namespace business.validata.com.Utils
{
    public class GenericLambdaExpressions : IGenericLambdaExpressions
    {
        protected readonly IMetadata metadata;
        public GenericLambdaExpressions(IMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);
            this.metadata = metadata;

        }

        

        public Expression<Func<TEntity, bool>> GetEntityByPrimaryKey<TEntity>(TEntity entity)
        {
            IProperty pkProperty = this.metadata.Entities[typeof(TEntity)].PrimaryKey;
            object? pkValue = pkProperty.PropertyInfo.GetValue(entity);

            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "e");

            BinaryExpression binaryExpression = EqualExpression(parameterExpression, pkProperty.PropertyInfo.Name, (int)pkValue!, pkProperty.PropertyInfo.PropertyType);

            BinaryExpression deletedOnBinaryExpression = DeletedOnBinaryExpression(parameterExpression);

            BinaryExpression filterBinaryExpression = Expression.And(binaryExpression, deletedOnBinaryExpression);

            return Expression.Lambda<Func<TEntity, bool>>(filterBinaryExpression, parameterExpression);
        }

        public Expression<Func<TEntity, bool>> GetEntityById<TEntity>(int id)
        {
            IProperty pkProperty = this.metadata.Entities[typeof(TEntity)].PrimaryKey;
            var fieldName = pkProperty.PropertyInfo.Name;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "e");

            BinaryExpression binaryExpression = EqualExpression(parameterExpression, fieldName, id, typeof(int));

            BinaryExpression deletedOnBinaryExpression = DeletedOnBinaryExpression(parameterExpression);

            BinaryExpression filterBinaryExpression = Expression.And(binaryExpression, deletedOnBinaryExpression);

            return Expression.Lambda<Func<TEntity, bool>>(filterBinaryExpression, parameterExpression);
        }

        public Expression<Func<TEntity, bool>> GetEntityByUniqueValue<TEntity>(TEntity entity, string fieldName, string value, List<int> ids)
        {

            IProperty pkProperty = this.metadata.Entities[typeof(TEntity)].PrimaryKey;

            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "e");

            BinaryExpression binaryExpression = EqualExpression(parameterExpression, fieldName, value, GetFieldTypeByName<TEntity>(fieldName));

            BinaryExpression deletedOnBinaryExpression = DeletedOnBinaryExpression(parameterExpression);

            BinaryExpression filterBinaryExpression = Expression.And(binaryExpression, deletedOnBinaryExpression);



            Expression<Func<TEntity, bool>> filterLambda = Expression.Lambda<Func<TEntity, bool>>(filterBinaryExpression, parameterExpression);

            Expression<Func<TEntity, bool>> notContains = ObjectUtil.NotContainsLambdaExpression<TEntity>(ids, pkProperty.PropertyInfo.Name);

            return ObjectUtil.ConcatLambdaExpression(filterLambda, notContains);
        }

        private BinaryExpression DeletedOnBinaryExpression(Expression parameterExpression)
        {
            MemberExpression deletedOnMemberExpression = Expression.Property(parameterExpression, nameof(BaseEntity.DeletedOn));
            return Expression.Equal(deletedOnMemberExpression, Expression.Constant(null, typeof(DateTime?)));
        }

        private BinaryExpression EqualExpression(Expression parameterExpression, string fieldname, dynamic? value, Type type)
        {
            MemberExpression memberExpression = Expression.Property(parameterExpression, fieldname);
            if (memberExpression.Member is PropertyInfo && Nullable.GetUnderlyingType(type) == null && type.IsValueType)
            {
                // type is non nullable
                var nonNullableValue = value != null ? Convert.ChangeType(value, type) : GetDefaultValue(type);

                return Expression.Equal(memberExpression, Expression.Constant(nonNullableValue, type));

            }
            return Expression.Equal(memberExpression, Expression.Constant(value, type));
        }

        private BinaryExpression NotEqualExpression<T>(Expression parameterExpression, string fieldname, T value)
        {

            MemberExpression memberExpression = Expression.Property(parameterExpression, fieldname);

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                var type = propertyInfo.PropertyType;

                if (Nullable.GetUnderlyingType(type) == null && type.IsValueType)
                {
                    // type is non nullable
                    var nonNullableValue = value != null ? Convert.ChangeType(value, type) : default(T);

                    return Expression.NotEqual(memberExpression, Expression.Constant(nonNullableValue, type));

                }
            }

            return Expression.NotEqual(memberExpression, Expression.Constant(value, typeof(T)));
        }

        public static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private Type GetFieldTypeByName<TEntity>(string fieldName)
        {
            IProperty property = this.metadata.Entities[typeof(TEntity)].Properties[fieldName];
            return property.PropertyInfo.PropertyType;
        }
    }
}
