using model.validata.com.ValueObjects.OperationSource;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace model.validata.com.Entities
{
    [Table(nameof(OperationSource), Schema = Constants.DefaultSchema)]
    public class OperationSource
    {

        
        [Key]
        public int OperationSourceId { get; set; }
        public OperationSourceName? Name { get; set; } 
    }
}