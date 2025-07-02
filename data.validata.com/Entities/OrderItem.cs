using data.validata.com.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace data.validata.com.Entities
{
    [Table(nameof(OrderItem), Schema = ValidataDbContext.DefaultSchema)]
    public class OrderItem : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
