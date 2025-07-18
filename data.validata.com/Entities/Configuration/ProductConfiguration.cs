﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using model.validata.com.Entities;
using util.validata.com;

namespace data.validata.com.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(b => b.Name).IsRequired(true).HasMaxLength(128).HasColumnType("nvarchar");
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