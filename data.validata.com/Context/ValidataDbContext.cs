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

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultSchema);
            SetOperationSource(modelBuilder);
            SetCustomer(modelBuilder);  

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
            IContexHelper<Customer> contexHelper = new ContexHelper<Customer>(
                this,
                new CustomerConfiguration(),
                DefaultSchema
                );
            contexHelper.Set(modelBuilder, null);
        }




    }
}