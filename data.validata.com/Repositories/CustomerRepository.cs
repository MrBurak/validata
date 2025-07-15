using Dapper;
using data.validata.com.Context;
using data.validata.com.Interfaces.Repository;
using model.validata.com;
using model.validata.com.DTO;
using model.validata.com.Entities;



namespace data.validata.com.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private string defaultSchema = Constants.DefaultSchema;
        private readonly QueryContext context;
        public CustomerRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<CustomerDto?> GetByIdAsync(int customerId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.Customer WHERE DeletedOn is null and CustomerId = @CustomerId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<CustomerDto>(sql, new { CustomerId = customerId });
            }
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync(PaginationRequest paginationRequest)
        {
            var sql = $@"
            SELECT * FROM {defaultSchema}.Customer 
            WHERE DeletedOn IS NULL
            ORDER BY CustomerId
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (var connection = this.context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Offset", paginationRequest.offset);
                parameters.Add("@PageSize", paginationRequest.pageSize);
                return await connection.QueryAsync<CustomerDto>(sql, parameters);
            }
        }



    }
}
