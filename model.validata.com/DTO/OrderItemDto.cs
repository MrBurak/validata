using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.DTO
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
