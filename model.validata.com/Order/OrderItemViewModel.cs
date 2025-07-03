using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.validata.com.Order
{
    public class OrderItemViewModel
    {
        public int Quantity { get; set; }

        public float ProductPrice { get; set; }

        public float TotalPrice { get { return Quantity * ProductPrice; } }

        public string? ProductName { get; set; }

    }
}
