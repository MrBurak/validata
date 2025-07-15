using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.Product
{
    public class ProductPrice : ValueObject
    {
        public decimal Value { get; } 

        private ProductPrice() { }

        public ProductPrice(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Product price must be equal or greater than zero.");
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

       

        public static implicit operator decimal(ProductPrice price) => price.Value;
        public static explicit operator ProductPrice(decimal value) => new ProductPrice(value);
    }
}
