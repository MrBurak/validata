using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.Order
{
    public class TotalAmount : ValueObject
    {
        public decimal Value { get; } 

        private TotalAmount() { }

        public TotalAmount(decimal value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Total amount must be greater than zero.");
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
