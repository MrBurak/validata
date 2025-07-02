using data.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using util.validata.com;

namespace Dayforce.PositionClassificationService.Data.Entities.Configuration
{
    [ExcludeFromCodeCoverage]
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasOne(o => o.Product).WithMany().IsRequired(true).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
            
            builder
                .HasOne(e => e.Order)
                .WithMany(e => e.OrderItems)
                .IsRequired(true)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.CreatedOnTimeStamp).IsRequired(true).HasColumnType(DateTimeUtil.DbDate_DataType);
            builder.Property(b => b.LastModifiedTimeStamp).IsRequired(true).HasColumnType(DateTimeUtil.DbDate_DataType);
            builder
                .HasOne(e => e.OperationSource)
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(e => e.OperationSourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}