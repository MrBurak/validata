using System.Linq.Expressions;


namespace data.validata.com.Context
{
    public class TEntityIndex<TEntity>
    {
        public Expression<Func<TEntity, object?>>? Expression { get; set; }
        public bool IsUnique { get; set; }

        public string? Filter { get; set; }
    }
}
