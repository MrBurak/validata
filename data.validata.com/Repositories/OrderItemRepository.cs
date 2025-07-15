using Dapper;
using data.validata.com.Context;
using data.validata.com.Interfaces.Repository;
using model.validata.com.DTO;



namespace data.validata.com.Repositories
{

    public class OrderItemRepository : IOrderItemRepository
    {
        private string defaultSchema = model.validata.com.Constants.DefaultSchema;
        private readonly QueryContext context;
        public OrderItemRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<OrderItemDto?> GetByIdAsync(int orderItemId, int orderId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.[Order] WHERE DeletedOn is null and OrderId = @OrderId and OrderItemId = @OrderItemId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<OrderItemDto>(sql, new { OrderId = orderId, OrderItemId = orderItemId });
            }
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync(int orderId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.OrderItem WHERE DeletedOn is null and OrderId = @OrderId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<OrderItemDto>(sql, new { OrderId = orderId });
            }
        }

       
    }
}
