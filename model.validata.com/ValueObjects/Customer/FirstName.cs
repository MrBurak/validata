using model.validata.com.ValueObjects.Base;

namespace model.validata.com.ValueObjects.Customer
{

    public class FirstName : ValueObject
    {
        public string Value { get; }

        public FirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("First name cannot be empty.", nameof(value));
            }
            if (value.Length > 128)
            {
                throw new ArgumentException("First name cannot exceed 128 characters.", nameof(value));
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
