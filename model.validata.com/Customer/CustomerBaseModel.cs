using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Customer
{
    public class CustomerBaseModel
    {

        [Required]
        [MaxLength(128)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(512)]
        public string? Address { get; set; }
        [Required]
        [MaxLength(10)]
        public string? Pobox { get; set; }
    }
}
