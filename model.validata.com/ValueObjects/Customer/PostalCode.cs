using model.validata.com.ValueObjects.Base;

namespace model.validata.com.ValueObjects.Customer
{
    
    public class PostalCode : ValueObject
    {
        public string Value { get; }

        public PostalCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Pobox cannot be empty.", nameof(value));
            }
            if (value.Length > 10)
            {
                throw new ArgumentException("Pobox cannot exceed 10 characters.", nameof(value));
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9\s-]+$"))
            {
                throw new ArgumentException("Pobox contains invalid characters.", nameof(value));
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
