using Dapper;
using System.Diagnostics.CodeAnalysis;
using data.validata.com.Interfaces.Repository;

using data.validata.com.Context;
using model.validata.com;
using model.validata.com.DTO;


namespace data.validata.com.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OrderRepository : IOrderRepository
    {
        private string defaultSchema = Constants.DefaultSchema;
        private readonly QueryContext context;
        public OrderRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<OrderDto?> GetByIdAsync(int orderId, int customerId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.[Order] WHERE DeletedOn is null and OrderId = @OrderId and CustomerId = @CustomerId order by OrderDate desc";
            using (var connection = this.context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OrderId", orderId);
                parameters.Add("@CustomerId", customerId);
                return await connection.QueryFirstOrDefaultAsync<OrderDto>(sql, parameters);
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync(int customerId, PaginationRequest paginationRequest)
        {
            

            var sql = $@"
            SELECT * FROM {defaultSchema}.[Order]
            WHERE DeletedOn IS NULL AND CustomerId = @CustomerId
            ORDER BY OrderId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            using (var connection = this.context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustomerId", customerId);
                parameters.Add("@Offset", paginationRequest.offset);
                parameters.Add("@PageSize", paginationRequest.pageSize);
                return await connection.QueryAsync<OrderDto>(sql, parameters);
            }
        }
        



    }
}
