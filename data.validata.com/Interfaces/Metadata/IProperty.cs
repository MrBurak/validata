using System.Reflection;

namespace data.validata.com.Interfaces.Metadata
{
    public interface IProperty
    {
        PropertyInfo PropertyInfo { get; }
        bool IsPrimaryKey { get; }
        bool IsEntityType { get; }
    }
}