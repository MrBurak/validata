using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using model.validata.com.Entities;

namespace data.validata.com.Entities.Configuration
{
    public class OperationSourceConfiguration : IEntityTypeConfiguration<OperationSource>
    {
        public void Configure(EntityTypeBuilder<OperationSource> builder)
        {
            builder.Property(b => b.OperationSourceId).UseIdentityColumn();
            builder.Property(b => b.Name).HasMaxLength(50).IsRequired(true);
        }
    }
}
