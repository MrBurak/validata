using data.validata.com.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace data.validata.com.Entities
{
    [Table(nameof(Order), Schema = ValidataDbContext.DefaultSchema)]
    public class Order : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
