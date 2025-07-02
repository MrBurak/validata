using business.validata.com.Interfaces;
using data.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using System.Linq.Expressions;
using System.Reflection;


namespace business.validata.com.Utils
{
    public class GenericLambdaExpressions : IGenericLambdaExpressions
    {
        protected readonly IMetadata metadata;
        public GenericLambdaExpressions(IMetadata metadata)
        {
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

        public static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
