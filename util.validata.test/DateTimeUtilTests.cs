using util.validata.com;

namespace util.validata.test
{
    public class DateTimeUtilTests
    {
        [Fact]
        public void SystemTime_ReturnsUtcNow()
        {
            var time1 = DateTimeUtil.SystemTime;
            System.Threading.Thread.Sleep(1);
            var time2 = DateTimeUtil.SystemTime;

            Assert.True(Math.Abs((time1 - DateTime.UtcNow).TotalMilliseconds) < 100);
            Assert.True(Math.Abs((time2 - DateTime.UtcNow).TotalMilliseconds) < 100);

            Assert.True(time2 >= time1);
        }

        [Fact]
        public void DbDate_DataType_ReturnsCorrectString()
        {
            Assert.Equal("datetime2(3)", DateTimeUtil.DbDate_DataType);
        }
    }
}
