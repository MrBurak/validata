using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using data.validata.com.Context;

namespace data.validata.com.Entities
{
    [Table(nameof(OperationSource), Schema = ValidataDbContext.DefaultSchema)]
    public class OperationSource
    {
        [Key]
        public int OperationSourceId { get; set; }
        public string? Name { get; set; }
    }
}
