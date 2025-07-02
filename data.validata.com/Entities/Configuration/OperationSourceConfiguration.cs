using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace data.validata.com.Entities.Configuration
{
    [ExcludeFromCodeCoverage]
    public class OperationSourceConfiguration : IEntityTypeConfiguration<OperationSource>
    {
        public void Configure(EntityTypeBuilder<OperationSource> builder)
        {
            builder.Property(b => b.OperationSourceId).UseIdentityColumn();
            builder.Property(b => b.Name).HasMaxLength(50).IsRequired(true);
        }
    }
}
