using Dapper;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Entities;
using data.validata.com.Context;
using model.validata.com;


namespace data.validata.com.Repositories
{
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
                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { ProductId });
            }
        }

       

        public async Task<IEnumerable<Product>> GetAllAsync(PaginationRequest paginationRequest)
        {
            var sql = $@"
            SELECT * FROM {defaultSchema}.Product
            WHERE DeletedOn IS NULL
            ORDER BY ProductId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (var connection = this.context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Offset", paginationRequest.offset);
                parameters.Add("@PageSize", paginationRequest.pageSize);
                return await connection.QueryAsync<Product>(sql, parameters);
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
