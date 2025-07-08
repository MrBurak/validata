using data.validata.com.Interfaces.Entities.Seed;
using Microsoft.EntityFrameworkCore;
using model.validata.com.Enumeration;
using util.validata.com;

namespace data.validata.com.Entities.Seed
{
    public class OperationSourceEntitySeed : IEntitySeed
    {
        public void Invoke(ModelBuilder modelBuilder)
        {
            foreach (var item in EnumUtil.EnumToList<BusinessOperationSource>())
            {
                modelBuilder.Entity<OperationSource>().HasData(
                new OperationSource
                {
                    OperationSourceId = Convert.ToInt32(item.Value),
                    Name = item.DisplayName
                });
            }

        }
    }
}