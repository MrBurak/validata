using System.ComponentModel.DataAnnotations;


namespace model.validata.com.Product
{
    public class ProductBaseModel
    {
        
        [Required]
        [MaxLength(128)]
        public string? Name { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }
    }
}
