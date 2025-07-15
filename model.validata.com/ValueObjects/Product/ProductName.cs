using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.Product
{
    public class ProductName : ValueObject
    {
        public string Value { get; }

        private ProductName() { }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(value));
            }
            if (value.Length > 128) 
            {
                throw new ArgumentException("Product name cannot exceed 128 characters.", nameof(value));
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(ProductName name) => name.Value;
        public static explicit operator ProductName(string value) => new ProductName(value);
    }
}
