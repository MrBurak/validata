using model.validata.com.ValueObjects.Base;

namespace model.validata.com.ValueObjects.Customer
{
   
    public class EmailAddress : ValueObject
    {
        public string Value { get; }

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email address cannot be empty.", nameof(value));
            }
            if (value.Length > 128)
            {
                throw new ArgumentException("Email address cannot exceed 128 characters.", nameof(value));
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Invalid email address format.", nameof(value));
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
