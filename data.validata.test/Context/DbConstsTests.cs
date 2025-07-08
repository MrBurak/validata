using data.validata.com.Context;

namespace data.validata.test.Context
{
    public class DbConstsTests
    {
        [Fact]
        public void DefaultSchema_ShouldBeValidata()
        {
            Assert.Equal("Validata", DbConsts.DefaultSchema);
        }
    }
}
