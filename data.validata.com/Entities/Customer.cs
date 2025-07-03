using data.validata.com.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace data.validata.com.Entities
{
    [Table(nameof(Customer), Schema = CommandContext.DefaultSchema)]
    public class Customer : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(128)]
        public string? FirstName { get; set; }
        [Required]
        [MaxLength(128)]
        public string? LastName { get; set; }
        [Required]
        [MaxLength(128)]
        public string? Email { get; set; }
        [Required]
        [MaxLength(512)]
        public string? Address { get; set; }
        [Required]
        [MaxLength(10)]
        public string? Pobox { get; set; }

    }
}
