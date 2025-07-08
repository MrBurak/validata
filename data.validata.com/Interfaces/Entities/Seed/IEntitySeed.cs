using Microsoft.EntityFrameworkCore;


namespace data.validata.com.Interfaces.Entities.Seed
{
    public interface IEntitySeed
    {
        void Invoke(ModelBuilder modelBuilder);
    }
}