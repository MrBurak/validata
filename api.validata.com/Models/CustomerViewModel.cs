using System.ComponentModel.DataAnnotations;

namespace customer.validata.com.Models
{
    public class CustomerViewModel : CustomerBaseModel
    {
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(128)]
        public string? Email { get; set; }

    }
}
