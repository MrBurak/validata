

using model.validata.com.Enumeration;

namespace model.validata.com.Validators
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<ValidationCode> Errors { get; } = new();

        public void AddError(ValidationCode message) => Errors.Add(message);
    }
}
