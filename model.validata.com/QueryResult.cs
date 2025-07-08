namespace model.validata.com
{
    public class QueryResult<T>
    {
        public T? Result { get; set; }
        public bool Success { get; set; }
        public string? Exception { get; set; }

    }
}
