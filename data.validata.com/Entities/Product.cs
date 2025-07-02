using data.validata.com.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace data.validata.com.Entities
{
    [Table(nameof(Product), Schema = ValidataDbContext.DefaultSchema)]
    public class Product : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(128)]
        public string? Name { get; set; }
        [Required]
        public float? Price { get; set; }
    }
}
