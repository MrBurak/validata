namespace model.validata.com.Order
{
    public class OrderItemViewModel
    {
        public int Quantity { get; set; }

        public float ProductPrice { get; set; }

        public float TotalAmount { get { return Quantity * ProductPrice; } }

        public string? ProductName { get; set; }

    }
}
