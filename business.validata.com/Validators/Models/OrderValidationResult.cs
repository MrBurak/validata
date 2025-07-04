using data.validata.com.Entities;
using model.validata.com.Validators;


namespace business.validata.com.Validators.Models
{
    public class OrderValidationResult
    {
        public ValidationResult<Order> ValidationResult { get; set; } = new ValidationResult<Order>();
        public List<Product> Products { get; set; }=new List<Product>();

    }
}
