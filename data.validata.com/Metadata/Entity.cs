using data.validata.com.Interfaces.Metadata;


namespace data.validata.com.Metadata
{
    public class Entity : IEntity
    {
        public Type Type { get; }
        public IReadOnlyDictionary<string, IProperty> Properties { get; }
        public IProperty PrimaryKey { get; }

        public Entity(Type type, IReadOnlyDictionary<string, IProperty> properties)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(properties);

            Type = type;
            Properties = properties;

            PrimaryKey = FindPrimaryKey();

        }

        private IProperty FindPrimaryKey()
        {
            return Properties[$"{Type.Name}Id"];
        }

        
    }
}