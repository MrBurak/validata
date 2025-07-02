using data.validata.com.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using util.validata.com;

namespace Dayforce.PositionClassificationService.Data.Entities.Configuration
{
    [ExcludeFromCodeCoverage]
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

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