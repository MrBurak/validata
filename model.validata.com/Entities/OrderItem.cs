using model.validata.com.ValueObjects.Product;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using util.validata.com;
using model.validata.com.ValueObjects.OrderItem;

namespace model.validata.com.Entities
{
    [Table(nameof(OrderItem), Schema = Constants.DefaultSchema)]
    public class OrderItem : BaseEntity
    {
        public OrderItem() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; private set; } 

        [Required]
        public int QuantityValue { get; private set; } 
        [NotMapped] 
        public ItemProductQuantity Quantity { get; private set; } 

        [Required]
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal ProductPriceValue { get; private set; } 
        [NotMapped]
        public ItemProductPrice ProductPrice { get; private set; } 

        [Required]
        public int ProductId { get; private set; } 
        [Required]
        public int OrderId { get; private set; } 

        public virtual Product Product { get; private set; } = null!; 
        public virtual Order Order { get; private set; } = null!; 
        public OrderItem(
            int productId,
            int orderId,
            ItemProductQuantity quantity,
            ItemProductPrice productPrice) : this() 
        {
            if (productId < 0) throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be positive.");
            if (orderId < 0) throw new ArgumentOutOfRangeException(nameof(orderId), "Order ID must be positive.");

            ProductId = productId;
            OrderId = orderId;

            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity), "Quantity cannot be null.");
            ProductPrice = productPrice ?? throw new ArgumentNullException(nameof(productPrice), "Product price cannot be null.");

            QuantityValue = quantity.Value;
            ProductPriceValue = productPrice.Value;

            CreatedOnTimeStamp = DateTimeUtil.SystemTime;
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;
        }

        
        public void UpdateQuantity(ItemProductQuantity newQuantity)
        {
            Quantity = newQuantity ?? throw new ArgumentNullException(nameof(newQuantity), "New quantity cannot be null.");

            QuantityValue = newQuantity.Value;

            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }

      
        public void UpdateProductPrice(ItemProductPrice newPrice)
        {
            ProductPrice = newPrice ?? throw new ArgumentNullException(nameof(newPrice), "New product price cannot be null.");

            ProductPriceValue = newPrice.Value;

            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }

      
      

        public void LoadValueObjectsFromBackingFields()
        {
            if (Quantity == null! && QuantityValue != default)
            {
                Quantity = new ItemProductQuantity(QuantityValue);
            }
            if (ProductPrice == null! && ProductPriceValue != default)
            {
                ProductPrice = new ItemProductPrice(ProductPriceValue);
            }
        }
    }
}
