using Dapper;
using System.Diagnostics.CodeAnalysis;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Entities;
using data.validata.com.Context;


namespace data.validata.com.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OrderItemRepository : IOrderItemRepository
    {
        private string defaultSchema = DbConsts.DefaultSchema;
        private readonly QueryContext context;
        public OrderItemRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<OrderItem?> GetByIdAsync(int orderItemId, int orderId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.Order WHERE DeletedOn is null and OrderId = @OrderId and OrderItemId = @OrderItemId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { OrderId = orderId, OrderItemId = orderItemId });
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync(int orderId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.OrderItem WHERE DeletedOn is null and OrderId = @OrderId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
            }
        }

       
    }
}
