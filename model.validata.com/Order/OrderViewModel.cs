

using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Order
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public int ProductCount { get; set; }

    }
}
