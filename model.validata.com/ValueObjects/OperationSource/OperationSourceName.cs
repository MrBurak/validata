using model.validata.com.ValueObjects.Base;


namespace model.validata.com.ValueObjects.OperationSource
{
    public class OperationSourceName : ValueObject
    {
        public string Value { get; }

        public OperationSourceName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");
            
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
