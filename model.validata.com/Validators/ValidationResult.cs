namespace model.validata.com.Validators
{
    public class ValidationResult<TEntity>
    {
        public TEntity? Entity { get; set; }
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new();

        public void AddError(string? message) 
        {
            if(string.IsNullOrWhiteSpace(message)) return;
            Errors.Add(message);
        }
    }
}
