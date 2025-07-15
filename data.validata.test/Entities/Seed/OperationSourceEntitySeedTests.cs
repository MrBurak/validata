using model.validata.com.Entities;
using data.validata.com.Entities.Seed;
using Microsoft.EntityFrameworkCore;
using model.validata.com.Enumeration;
using util.validata.com;

namespace data.validata.test.Entities.Seed
{
    public class OperationSourceEntitySeedTests
    {
        [Fact]
        public void Invoke_ShouldAddSeedDataToModelBuilder()
        {
            var modelBuilder = new ModelBuilder();
            var seed = new OperationSourceEntitySeed();

            seed.Invoke(modelBuilder);

            var entity = modelBuilder.Model.FindEntityType(typeof(OperationSource));
            var seededData = entity!.GetSeedData().ToList();

            var enumList = EnumUtil.EnumToList<BusinessOperationSource>().ToList();

            Assert.Equal(enumList.Count, seededData.Count);

            Assert.NotNull(enumList.FirstOrDefault(x=> (BusinessOperationSource)x.Value! == BusinessOperationSource.PreDefined));
            Assert.NotNull(enumList.FirstOrDefault(x => (BusinessOperationSource)x.Value! == BusinessOperationSource.Api));
            Assert.NotNull(enumList.FirstOrDefault(x => (BusinessOperationSource)x.Value! == BusinessOperationSource.Import));


        }
    }
}
