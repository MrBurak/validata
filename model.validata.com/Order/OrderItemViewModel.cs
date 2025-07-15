namespace model.validata.com.Order
{
    public class OrderItemViewModel
    {
        public int Quantity { get; set; }

        public decimal ProductPrice { get; set; }

        public decimal TotalAmount { get { return Quantity * ProductPrice; } }

        public string? ProductName { get; set; }

    }
}
