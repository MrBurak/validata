using Microsoft.EntityFrameworkCore;


namespace data.validata.com.Entities.Seed.Interface
{
    public interface IEntitySeed
    {
        void Invoke(ModelBuilder modelBuilder);
    }
}