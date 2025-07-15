using data.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using model.validata.com.Entities;
using System.Reflection;

namespace data.validata.com.Metadata
{
    public class Property : IProperty
    {
        public PropertyInfo PropertyInfo { get; }
        public bool IsPrimaryKey { get; }
        public bool IsEntityType { get; }

        public Property(PropertyInfo propertyInfo, bool isPrimaryKey)
        {
            ArgumentNullException.ThrowIfNull(propertyInfo);

            PropertyInfo = propertyInfo;
            IsPrimaryKey = isPrimaryKey;

            IsEntityType = propertyInfo.PropertyType
                .IsSubclassOf(typeof(BaseEntity));
        }
    }
}