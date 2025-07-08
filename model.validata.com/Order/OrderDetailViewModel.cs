namespace model.validata.com.Order
{
    public class OrderDetailViewModel:OrderViewModel
    {


        public IEnumerable<OrderItemViewModel> Items { get; set;} = new List<OrderItemViewModel>();

        
    }
}
