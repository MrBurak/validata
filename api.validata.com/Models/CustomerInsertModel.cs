using System.ComponentModel.DataAnnotations;

namespace customer.validata.com.Models
{
    public class CustomerInsertModel : CustomerBaseModel
    {
        [Required]
        [MaxLength(128)]
        public string? Email { get; set; }

    }
}
