using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Order
{
    public class OrderDetailViewModel:OrderViewModel
    {


        public IEnumerable<OrderItemViewModel> Items { get; set;} = new List<OrderItemViewModel>();

        
    }
}
