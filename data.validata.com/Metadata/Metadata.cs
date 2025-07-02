using data.validata.com.Interfaces.Metadata;


namespace data.validata.com.Metadata
{
    public class Metadata : IMetadata
    {
        public IReadOnlyDictionary<Type, IEntity> Entities { get; }

        public Metadata(IReadOnlyDictionary<Type, IEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            Entities = entities;
        }
    }
}