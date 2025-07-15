using model.validata.com.ValueObjects.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using util.validata.com;


namespace model.validata.com.Entities
{
    [Table(nameof(Order), Schema = Constants.DefaultSchema)]
    public class Order : BaseEntity
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; private set; }

        [Required]
        public DateTime OrderDateValue { get; private set; }
        [NotMapped]
        public DateTime OrderDate { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmountValue { get; private set; }
        [NotMapped]
        public TotalAmount TotalAmount { get; private set; }


        [Required]
        public int ProductQuantityValue { get; private set; }
        [NotMapped]
        public ProductQuantity ProductQuantity { get; private set; }

        [Required]
        public int CustomerId { get; private set; }

        public virtual Customer Customer { get; private set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; private set; }


        public Order(
            int orderId,
            int customerId,
            DateTime orderDate,
            TotalAmount totalAmount,
            ProductQuantity productQuantity) : this()
        {
            if (customerId < 0) throw new ArgumentOutOfRangeException(nameof(customerId), "Customer ID must be positive.");
            if (orderDate == default) throw new ArgumentException("Order date cannot be default.", nameof(orderDate));

            CustomerId = customerId;
            OrderDate = orderDate; 
            OrderDateValue = orderDate; 
            OrderId = orderId;

            TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount), "Order total amount cannot be null.");
            ProductQuantity = productQuantity ?? throw new ArgumentNullException(nameof(productQuantity), "Product quantity cannot be null.");

            TotalAmountValue = totalAmount.Value;
            ProductQuantityValue = productQuantity.Value;

            CreatedOnTimeStamp = DateTimeUtil.SystemTime;
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;
        }

        
        public void UpdateTotalAmount(TotalAmount newTotalAmount)
        {
            TotalAmount = newTotalAmount ?? throw new ArgumentNullException(nameof(newTotalAmount), "New total amount cannot be null.");

            TotalAmountValue = newTotalAmount.Value;

            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }


        public void UpdateProductCount(ProductQuantity newProductCount)
        {
            ProductQuantity = newProductCount ?? throw new ArgumentNullException(nameof(newProductCount), "New product count cannot be null.");

            ProductQuantityValue = newProductCount.Value;

            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }

  
        public void AddOrderItem(OrderItem orderItem)
        {
            if (orderItem == null) throw new ArgumentNullException(nameof(orderItem));
            OrderItems.Add(orderItem);
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;
        }

        

        public void LoadValueObjectsFromBackingFields()
        {
            if (OrderDate == default && OrderDateValue != default)
            {
                OrderDate = OrderDateValue;
            }

            if (TotalAmount == null! && TotalAmountValue != default) 
            {
                TotalAmount = new TotalAmount(TotalAmountValue);
            }
            if (ProductQuantity == null! && ProductQuantityValue != default) 
            {
                ProductQuantity = new ProductQuantity(ProductQuantityValue);
            }
        }
    }
}