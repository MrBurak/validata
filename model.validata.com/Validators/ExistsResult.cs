

namespace model.validata.com.Validators
{
    public class ExistsResult<TEntity>
    {
        public TEntity? Entity { get; set; }
        public string? Code
        { 
            get 
            { 
                return Entity == null ? "No record found"! : null; 
            } 
        }
    }
}
