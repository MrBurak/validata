using model.validata.com.ValueObjects.Base;

namespace model.validata.com.ValueObjects.Customer
{

    public class LastName : ValueObject
    {
        public string Value { get; }

        public LastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Last name cannot be empty.", nameof(value));
            }
            if (value.Length > 128)
            {
                throw new ArgumentException("Last name cannot exceed 128 characters.", nameof(value));
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
