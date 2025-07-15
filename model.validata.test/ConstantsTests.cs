using model.validata.com;

namespace model.validata.test
{
    public class ConstantsTests
    {
        [Fact]
        public void DefaultSchema_ShouldBeValidata()
        {
            Assert.Equal("Validata", Constants.DefaultSchema);
        }
    }
}
