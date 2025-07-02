using data.validata.com.Entities;
using data.validata.com.Interfaces.Metadata;
using data.validata.com.Metadata;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace data.validata.com.Metadata
{
    public static class MetadataFactory
    {
        public static Metadata Create()
        {
            ReadOnlyDictionary<Type, IEntity> entities = CreateEntities();

            return new Metadata(entities);
        }

        private static ReadOnlyDictionary<Type, IEntity> CreateEntities()
        {
            Type baseType = typeof(BaseEntity);

            Dictionary<Type, IEntity> entities = baseType.Assembly.ExportedTypes
                .Where(type => type.IsSubclassOf(baseType) && type.GetCustomAttributes(typeof(TableAttribute), false).Any())
                .Select(type => CreateEntity(type))
                .Cast<IEntity>()
                .ToDictionary(entity => entity.Type);

            return new ReadOnlyDictionary<Type, IEntity>(entities);
        }

        private static Entity CreateEntity(Type type)
        {

            Dictionary<string, IProperty> properties = type
                .GetProperties()
                .Select(pi => new Property(pi, IsPrimaryKey(pi, type)))
                .Cast<IProperty>()
                .ToDictionary(p => p.PropertyInfo.Name);

            return new Entity(type, new ReadOnlyDictionary<string, IProperty>(properties));
        }

        

        private static bool IsPrimaryKey(PropertyInfo propertyInfo, Type type)
        {
            return propertyInfo.Name == $"{type.Name}Id";
        }

        
    }
}