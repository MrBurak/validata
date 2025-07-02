using data.validata.com.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace data.validata.com.Entities
{
    [Table(nameof(Customer), Schema = ValidataDbContext.DefaultSchema)]
    public class Customer : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Addess { get; set; }

        public string? Pobox { get; set; }

    }
}
