using data.validata.com.Entities.Seed.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.validata.com.Context
{
    public interface IContexHelper<TEntity>
    {
        void Set(ModelBuilder modelBuilder, List<TEntityIndex<TEntity>>? indexes = null);
        void SetStandAlone(ModelBuilder modelBuilder, List<TEntityIndex<TEntity>>? indexes = null);
    }

    public class ContexHelper<TEntity> : IContexHelper<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly IEntityTypeConfiguration<TEntity> _configuration;
        private readonly IEntitySeed? _seed;
        private readonly string _schemaName;


        public ContexHelper(DbContext context, IEntityTypeConfiguration<TEntity> configuration, string schemaName, IEntitySeed? seed = null)
        {
            _schemaName = schemaName;
            _context = context;
            _configuration = configuration;
            _seed = seed;
        }

        public void Set(ModelBuilder modelBuilder, List<TEntityIndex<TEntity>>? indexes = null)
        {

            modelBuilder.Entity<TEntity>().ToTable(typeof(TEntity).Name, options =>
            {
                options.IsTemporal();
            });


            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    if (index.Expression == null) continue;
                    if (index.Filter == null)
                    {
                        modelBuilder.Entity<TEntity>().HasIndex(index.Expression).IsUnique(index.IsUnique);
                    }
                    else
                    {
                        modelBuilder.Entity<TEntity>().HasIndex(index.Expression).IsUnique(index.IsUnique).HasFilter(index.Filter);

                    }
                }
            }
            modelBuilder.ApplyConfiguration(_configuration);

            if (_seed == null) return;
            _seed.Invoke(modelBuilder);
        }

        public void SetStandAlone(ModelBuilder modelBuilder, List<TEntityIndex<TEntity>>? indexes = null)
        {

            modelBuilder.Entity<TEntity>().ToTable(typeof(TEntity).Name);


            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    if (index.Expression == null) continue;
                    if (index.Filter == null)
                    {
                        modelBuilder.Entity<TEntity>().HasIndex(index.Expression).IsUnique(index.IsUnique);
                    }
                    else
                    {
                        modelBuilder.Entity<TEntity>().HasIndex(index.Expression).IsUnique(index.IsUnique).HasFilter(index.Filter);

                    }
                }
            }
            modelBuilder.ApplyConfiguration(_configuration);

            if (_seed == null) return;
            _seed.Invoke(modelBuilder);
        }


    }
}