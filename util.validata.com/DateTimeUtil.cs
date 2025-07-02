using System.Diagnostics.CodeAnalysis;


namespace util.validata.com
{
    [ExcludeFromCodeCoverage]
    public class DateTimeUtil
    {
        public static DateTime SystemTime { get { return DateTime.UtcNow; } }
        public static string DbDate_DataType { get { return "datetime2(3)"; } }

    }
}
