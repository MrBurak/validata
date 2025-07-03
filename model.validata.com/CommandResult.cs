

namespace model.validata.com
{
    public class CommandResult<T> : QueryResult<T>
    {
        public List<string> Validations { get; set; } = new List<string>();

    }
}
