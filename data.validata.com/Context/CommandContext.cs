using data.validata.com.Configuration;
using data.validata.com.Entities.Configuration;
using data.validata.com.Entities.Seed;
using Microsoft.EntityFrameworkCore;
using model.validata.com.Entities;
using model.validata.com.ValueObjects.Customer;
using model.validata.com.ValueObjects.OperationSource;
using model.validata.com.ValueObjects.Order;
using model.validata.com.ValueObjects.OrderItem;
using model.validata.com.ValueObjects.Product;
using model.validata.com;

namespace data.validata.com.Context
{
    public partial class CommandContext : DbContext
    {
        public const string DefaultSchema = Constants.DefaultSchema;

        public CommandContext(DbContextOptions<CommandContext> options) : base(options) { }

        public virtual DbSet<OperationSource>? OperationSource { get; set; }
        public virtual DbSet<Customer> Customer { get; set; } = null!;
        public virtual DbSet<Order> Order { get; set; } = null!;
        public virtual DbSet<Product> Product { get; set; } = null!;

        public virtual DbSet<OrderItem> OrderItem { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            SetOperationSource(modelBuilder);
            SetCustomer(modelBuilder);  
            SetOrder(modelBuilder);
            SetProduct(modelBuilder);
            SetOrderItem(modelBuilder); 

        }

        

        private void SetOperationSource(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperationSource>()
           .Property(e => e.Name)
           .HasConversion(
               name => name!.Value,
               value => new OperationSourceName(value));

            IContexHelper<OperationSource> contexHelper = new ContexHelper<OperationSource>(
                this,
                new OperationSourceConfiguration(),
                DefaultSchema,
                new OperationSourceEntitySeed()
                );

            contexHelper.SetStandAlone(modelBuilder, null);
        }

        private void SetCustomer(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<Customer>()
            .Property(e => e.Email)
            .HasConversion(
                val => val.Value,        
                value => new EmailAddress(value));

            modelBuilder.Entity<Customer>()
            .Property(e => e.FirstName)
            .HasConversion(
                val => val.Value,
                value => new FirstName(value));

            modelBuilder.Entity<Customer>()
           .Property(e => e.LastName)
           .HasConversion(
               val => val.Value,
               value => new LastName(value));

            modelBuilder.Entity<Customer>()
           .Property(e => e.Address)
           .HasConversion(
               val => val.Value,
               value => new StreetAddress(value));

            modelBuilder.Entity<Customer>()
           .Property(e => e.Pobox)
           .HasConversion(
               val => val.Value,
               value => new PostalCode(value));

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Ignore(e => e.EmailValue);
                entity.Ignore(e => e.FirstNameValue);
                entity.Ignore(e => e.LastNameValue);
                entity.Ignore(e => e.AddressValue);
                entity.Ignore(e => e.PoboxValue);
            });



            string filter = "[Email] IS NOT NULL AND [DeletedOn] IS NULL";

            IContexHelper<Customer> contexHelper = new ContexHelper<Customer>(
                    this,
                    new CustomerConfiguration(),
                    DefaultSchema
                    );

            var indexes = new List<TEntityIndex<Customer>>
            {
                new TEntityIndex<Customer>
                {
                    Expression=p => new { p.Email, p.DeletedOn },
                    IsUnique=true,
                    Filter=filter
                }
            };

            contexHelper.Set(modelBuilder, indexes);
        }

        private void SetOrder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
              .Property(e => e.TotalAmount)
              .HasConversion(
                  val => val.Value,
                  value => new TotalAmount(value));

            modelBuilder.Entity<Order>()
              .Property(e => e.ProductQuantity)
              .HasConversion(
                  val => val.Value,
                  value => new ProductQuantity(value));


            modelBuilder.Entity<Order>(entity =>
            {
                entity.Ignore(e => e.OrderDateValue);
                entity.Ignore(e => e.ProductQuantityValue);
                entity.Ignore(e => e.TotalAmountValue);
            });


            IContexHelper<Order> contexHelper = new ContexHelper<Order>(
                this,
                new OrderConfiguration(),
                DefaultSchema
                );
            contexHelper.Set(modelBuilder, null);
        }

        private void SetProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
              .Property(e => e.Name)
              .HasConversion(
                  name => name!.Value,
                  value => new ProductName(value));

            modelBuilder.Entity<Product>()
              .Property(e => e.Price)
              .HasConversion(
                  name => name!.Value,
                  value => new ProductPrice(value));

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Ignore(e => e.NameValue);
                entity.Ignore(e => e.PriceValue);
            });


            string filter = "[Name] IS NOT NULL AND [DeletedOn] IS NULL";

            IContexHelper<Product> contexHelper = new ContexHelper<Product>(
                this,
                new ProductConfiguration(),
                DefaultSchema
                );

            var indexes = new List<TEntityIndex<Product>>
            {
                new TEntityIndex<Product>
                {
                    Expression=p => new { p.Name, p.DeletedOn },
                    IsUnique=true,
                    Filter=filter
                } 
            };

            contexHelper.Set(modelBuilder, indexes);
        }

        private void SetOrderItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
             .Property(e => e.Quantity)
             .HasConversion(
                 val => val.Value,
                 value => new ItemProductQuantity(value));

            modelBuilder.Entity<OrderItem>()
              .Property(e => e.ProductPrice)
              .HasConversion(
                  val => val.Value,
                  value => new ItemProductPrice(value));

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Ignore(e => e.QuantityValue);
                entity.Ignore(e => e.ProductPriceValue);
            });


            IContexHelper<OrderItem> contexHelper = new ContexHelper<OrderItem>(
                this,
                new OrderItemConfiguration(),
                DefaultSchema
                );
            contexHelper.Set(modelBuilder, null);
        }
    }
}