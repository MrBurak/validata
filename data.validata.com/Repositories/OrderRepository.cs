using Dapper;
using System.Diagnostics.CodeAnalysis;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Entities;
using data.validata.com.Context;


namespace data.validata.com.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OrderRepository : IOrderRepository
    {
        private string defaultSchema = DbConsts.DefaultSchema;
        private readonly QueryContext context;
        public OrderRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<Order?> GetByIdAsync(int orderId, int customerId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.[Order] WHERE DeletedOn is null and OrderId = @OrderId and CustomerId = @CustomerId order by OrderDate desc";
            using (var connection = this.context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OrderId", orderId);
                parameters.Add("@CustomerId", customerId);
                return await connection.QueryFirstOrDefaultAsync<Order>(sql, parameters);
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int customerId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.[Order] WHERE DeletedOn is null and CustomerId = @CustomerId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
            }
        }



    }
}
