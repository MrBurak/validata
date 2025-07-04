using Dapper;
using System.Diagnostics.CodeAnalysis;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Entities;
using data.validata.com.Context;


namespace data.validata.com.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProductRepository : IProductRepository
    {
        private string defaultSchema = DbConsts.DefaultSchema;
        private readonly QueryContext context;
        public ProductRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<Product?> GetByIdAsync(int ProductId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.Product WHERE DeletedOn is null and ProductId = @ProductId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductId = ProductId });
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {defaultSchema}.Product WHERE DeletedOn is null";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetAllWithDeletedAsync()
        {
            var sql = $"SELECT * FROM {defaultSchema}.Product";

            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(sql);
            }
        }
    }
}
