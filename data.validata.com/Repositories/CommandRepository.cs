using data.validata.com.Context;
using data.validata.com.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace data.validata.com.Repositories
{
    public class CommandRepository<T> : ICommandRepository<T>
        where T : class, new()
    {


        private CommandContext validataDbContext;

        public CommandRepository(CommandContext validataDbContext)
        {
            this.validataDbContext = validataDbContext;
        }
        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> query)
        {
            var entity = await validataDbContext.Set<T>()
                .FirstOrDefaultAsync(query);

            if (entity != null)
            {
                validataDbContext.Entry(entity).State = EntityState.Detached;
            }

            return await validataDbContext.Set<T>()
                .FirstOrDefaultAsync(query);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> query)
        {
            var entities = validataDbContext.Set<T>()
                .Where(query);

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    validataDbContext.Entry(entity).State = EntityState.Detached;
                }
            }

            return await validataDbContext.Set<T>()
                .Where(query)
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            validataDbContext.Set<T>()
                .Add(entity);

            await validataDbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> query)
        {
            IList<T> entites = await GetListAsync(query);

            validataDbContext.RemoveRange(entites);

            return validataDbContext.SaveChanges() != 0;
        }

        public async Task UpdateAsync(Expression<Func<T, bool>> filterExpression, List<Action<T>> properties)
        {
            var recordsToBeUpdated = await validataDbContext.Set<T>()
                .Where(filterExpression)
                .ToListAsync();

            foreach (var property in properties)
            {
                recordsToBeUpdated.ForEach(property);
            }
            await validataDbContext.SaveChangesAsync();
        }

       
    }
}