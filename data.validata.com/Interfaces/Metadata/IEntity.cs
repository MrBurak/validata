

namespace data.validata.com.Interfaces.Metadata
{
    public interface IEntity
    {
        Type Type { get; }
        IReadOnlyDictionary<string, IProperty> Properties { get; }
        
        IProperty PrimaryKey { get; }
    }
}