using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Order
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }

        List<OrderViewModel> Items { get; set;} = new List<OrderViewModel>();

        
    }
}
