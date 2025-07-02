using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace util.validata.com
{
    [ExcludeFromCodeCoverage]
    public static class EnumUtil
    {
        public static string? GetDisplayName(this Enum enumValue)
        {

            var typ = enumValue.GetType();
            if (typ == null) return string.Empty;
            var members = typ.GetMember(enumValue.ToString());
            if (members == null) return string.Empty;
            var member = members.First();
            if (member == null) return string.Empty;
            var attr = member.GetCustomAttribute<DisplayAttribute>();
            if (attr == null) return string.Empty;
            return attr.GetName();
        }
        public static string? GetDisplayDescription(this Enum enumValue)
        {

            var typ = enumValue.GetType();
            if (typ == null) return string.Empty;
            var members = typ.GetMember(enumValue.ToString());
            if (members == null) return string.Empty;
            var member = members.First();
            if (member == null) return string.Empty;
            var attr = member.GetCustomAttribute<DisplayAttribute>();
            if (attr == null) return string.Empty;
            return attr.GetDescription();
        }

        public static string? GetDisplayGroupName(this Enum enumValue)
        {

            var typ = enumValue.GetType();
            if (typ == null) return string.Empty;
            var members = typ.GetMember(enumValue.ToString());
            if (members == null) return string.Empty;
            var member = members.First();
            if (member == null) return string.Empty;
            var attr = member.GetCustomAttribute<DisplayAttribute>();
            if (attr == null) return string.Empty;
            return attr.GetGroupName();
        }
        public class NameValue
        {
            public string? DisplayGroupName { get; set; }
            public string? DisplayName { get; set; }
            public string? DisplayDescription { get; set; }
            public string? Name { get; set; }
            public object? Value { get; set; }



        }

        public static List<NameValue> EnumToList<T>()
        {
            List<NameValue> lst = new List<NameValue>();

            foreach (Enum en in Enum.GetValues(typeof(T)))
            {
                lst.Add(EnumToObject(en));

            }

            return lst;
        }
        public static NameValue EnumToObject(Enum en)
        {
            return new NameValue { Name = en.ToString(), Value = en.GetHashCode(), DisplayName = GetDisplayName(en), DisplayDescription = GetDisplayDescription(en), DisplayGroupName = GetDisplayGroupName(en) };
        }

        public static string FindGroupNameByValueId<T>(int value)
        {
            foreach (var item in EnumUtil.EnumToList<T>())
            {
                if (Convert.ToInt32(item.Value) == value)
                {
                    return item.DisplayGroupName!;
                }
            }
            throw new Exception($"Unsupported value for {typeof(T).FullName} : {value} ");
        }
    }
}
