using model.validata.com.ValueObjects.Base;


namespace model.validata.test.ValueObjects.Base
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            Amount = amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }
    }


    public class ValueObjectTests
    {
        [Fact]
        public void EqualObjects_ShouldBeEqual()
        {
            var money1 = new Money(100.00m);
            var money2 = new Money(100.00m);

            Assert.Equal(money1, money2);
            Assert.True(money1 == money2);
            Assert.False(money1 != money2);
        }

        [Fact]
        public void DifferentObjects_ShouldNotBeEqual()
        {
            var money1 = new Money(100.00m);
            var money2 = new Money(200.00m);

            Assert.NotEqual(money1, money2);
            Assert.False(money1 == money2);
            Assert.True(money1 != money2);
        }

        [Fact]
        public void ComparingWithNull_ShouldBehaveCorrectly()
        {
            Money money1 = new Money(50.00m);
            Money? money2 = null;

            Assert.False(money1 == money2);
            Assert.True(money1 != money2);
            Assert.False(money1.Equals(null));
        }

        [Fact]
        public void TwoNulls_ShouldBeEqual()
        {
            Money? money1 = null;
            Money? money2 = null;

            Assert.True(money1 == money2);
            Assert.False(money1 != money2);
        }

        [Fact]
        public void GetHashCode_ShouldMatchForEqualObjects()
        {
            var money1 = new Money(75.00m);
            var money2 = new Money(75.00m);

            Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ShouldDifferForDifferentObjects()
        {
            var money1 = new Money(75.00m);
            var money2 = new Money(80.00m);

            Assert.NotEqual(money1.GetHashCode(), money2.GetHashCode());
        }
    }

}
