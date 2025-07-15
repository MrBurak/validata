using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.OrderItem
{
    public class ItemProductPrice : ValueObject
    {
        public decimal Value { get; } 

        private ItemProductPrice() { }

        public ItemProductPrice(decimal value)
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
