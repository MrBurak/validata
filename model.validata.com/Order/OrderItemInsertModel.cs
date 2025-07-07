

using System.ComponentModel.DataAnnotations;

namespace model.validata.com.Order
{
    public class OrderItemInsertModel
    {

        [Required]
        [Range(1, int.MaxValue)]

        public int Quantity { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}
