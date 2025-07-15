using model.validata.com.ValueObjects.Product;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using util.validata.com;


namespace model.validata.com.Entities
{
    [Table(nameof(Product), Schema = Constants.DefaultSchema)]
    public class Product : BaseEntity
    {
        
        public Product() { }

        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; private set; } 


        [Required]
        [MaxLength(256)] 
        public string NameValue { get; private set; } 
        [NotMapped] 
        public ProductName Name { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] 
        public decimal PriceValue { get; private set; } 
        [NotMapped]
        public ProductPrice Price { get; private set; }

        
        public Product(
            int productId,
            ProductName name,
            ProductPrice price)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Product name cannot be null.");
            Price = price ?? throw new ArgumentNullException(nameof(price), "Product price cannot be null.");

            NameValue = name.Value;
            PriceValue = price.Value;
            ProductId=productId;

            CreatedOnTimeStamp = DateTimeUtil.SystemTime;
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;
        }


        public void ChangeName(ProductName newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName), "New product name cannot be null.");

            NameValue = newName.Value;

            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }

        
        public void UpdatePrice(ProductPrice newPrice)
        {
            Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice), "New product price cannot be null.");
            PriceValue = newPrice.Value;
            LastModifiedTimeStamp = DateTimeUtil.SystemTime;

        }

        
       

        
        public void LoadValueObjectsFromBackingFields()
        {
            if (Name == null! && NameValue != null)
            {
                Name = new ProductName(NameValue);
            }
            if (Price == null!) 
            {
                Price = new ProductPrice(PriceValue);
            }
        }
    }
}
