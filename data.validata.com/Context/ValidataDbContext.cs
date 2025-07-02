using data.validata.com.Entities;
using data.validata.com.Entities.Configuration;
using data.validata.com.Entities.Seed;
using Dayforce.PositionClassificationService.Data.Entities.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace data.validata.com.Context
{
    [ExcludeFromCodeCoverage]
    public partial class ValidataDbContext : DbContext
    {
        public const string DefaultSchema = "Validata";

        public ValidataDbContext(DbContextOptions<ValidataDbContext> options) : base(options) { }

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
            IContexHelper<Order> contexHelper = new ContexHelper<Order>(
                this,
                new OrderConfiguration(),
                DefaultSchema
                );
            contexHelper.Set(modelBuilder, null);
        }

        private void SetProduct(ModelBuilder modelBuilder)
        {
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
            IContexHelper<OrderItem> contexHelper = new ContexHelper<OrderItem>(
                this,
                new OrderItemConfiguration(),
                DefaultSchema
                );
            contexHelper.Set(modelBuilder, null);
        }
    }
}