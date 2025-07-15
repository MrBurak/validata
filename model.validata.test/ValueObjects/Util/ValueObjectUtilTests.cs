using model.validata.com.ValueObjects.Customer;
using model.validata.com.ValueObjects.Util;

namespace model.validata.test.ValueObjects.Util
{


    public class ValueObjectUtilTests
    {
        private class TestEntity
        {
            public FirstName FirstName { get; set; } = new FirstName("Burak");
            public LastName? LastName { get; set; }
            public string NotAValueObject { get; set; } = "Invalid";
        }

        [Fact]
        public void GetValue_ShouldReturnValue_WhenValid()
        {
            var entity = new TestEntity();
            var result = ValueObjectUtil.GetValue(entity, nameof(TestEntity.FirstName));
            Assert.Equal("Burak", result);
        }

        [Fact]
        public void GetValue_ShouldReturnEmpty_WhenFieldIsNull()
        {
            var entity = new TestEntity { LastName = null };
            var result = ValueObjectUtil.GetValue(entity, nameof(TestEntity.LastName));
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetValue_ShouldReturnEmpty_WhenFieldDoesNotExist()
        {
            var entity = new TestEntity();
            var result = ValueObjectUtil.GetValue(entity, "NonExistentField");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetValue_ShouldReturnEmpty_WhenEntityIsNull()
        {
            var result = ValueObjectUtil.GetValue<TestEntity>(null!, "FirstName");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetValue_ShouldReturnEmpty_WhenNotAValueObject()
        {
            var entity = new TestEntity();
            var result = ValueObjectUtil.GetValue(entity, nameof(TestEntity.NotAValueObject));
            Assert.Equal(string.Empty, result);
        }
    }

}
