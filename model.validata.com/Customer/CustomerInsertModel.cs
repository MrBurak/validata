using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Customer
{
    public class CustomerInsertModel : CustomerBaseModel
    {
        [Required]
        [MaxLength(128)]
        public string? Email { get; set; }

    }
}
