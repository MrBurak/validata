using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.Order
{


    public class ProductQuantity : ValueObject
    {
        public int Value { get; }

        private ProductQuantity() { }

        public ProductQuantity(int value)
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
