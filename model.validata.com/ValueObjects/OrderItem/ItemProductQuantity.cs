using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.OrderItem
{


    public class ItemProductQuantity : ValueObject
    {
        public int Value { get; }

        private ItemProductQuantity() { }

        public ItemProductQuantity(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Product quantity must be greater than zero.");
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
