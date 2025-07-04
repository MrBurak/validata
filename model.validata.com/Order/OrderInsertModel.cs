using System.ComponentModel.DataAnnotations;


namespace model.validata.com.Order
{
    public class OrderInsertModel
    {
        public int CustomerId { get; set; }

        [MinLength(1)]
        public IEnumerable<OrderItemInsertModel>? Items { get; set; }
    }
}
