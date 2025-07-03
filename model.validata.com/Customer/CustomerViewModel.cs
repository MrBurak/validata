using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Customer
{
    public class CustomerViewModel : CustomerBaseModel
    {
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(128)]
        public string? Email { get; set; }

    }
}
