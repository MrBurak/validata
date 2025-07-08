namespace util.validata.com
{
  
    public class DateTimeUtil
    {
        public static DateTime SystemTime { get { return DateTime.UtcNow; } }
        public static string DbDate_DataType { get { return "datetime2(3)"; } }

    }
}
