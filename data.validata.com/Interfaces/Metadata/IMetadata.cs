
namespace data.validata.com.Interfaces.Metadata
{
    public interface IMetadata
    {
        IReadOnlyDictionary<Type, IEntity> Entities { get; }
    }
}
