using System.ComponentModel.DataAnnotations;


namespace model.validata.com.Order
{
    public class OrderInsertModel
    {
        [Required]
        public int CustomerId { get; set; }

        [MinLength(1)]
        public IEnumerable<OrderItemInsertModel> Items { get; set; }=new List<OrderItemInsertModel>();
    }
}
