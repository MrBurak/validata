using model.validata.com.ValueObjects.Base;

namespace model.validata.com.ValueObjects.Customer
{

    public class StreetAddress : ValueObject
    {
        public string Value { get; }

        public StreetAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Address cannot be empty.", nameof(value));
            }
            if (value.Length > 512)
            {
                throw new ArgumentException("Address cannot exceed 512 characters.", nameof(value));
            }
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
