using Dapper;
using System.Diagnostics.CodeAnalysis;
using data.validata.com.Interfaces.Repository;
using data.validata.com.Entities;
using data.validata.com.Context;


namespace data.validata.com.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CustomerRepository : ICustomerRepository
    {
        private string defaultSchema = DbConsts.DefaultSchema;
        private readonly QueryContext context;
        public CustomerRepository(QueryContext context)
        {
            this.context = context;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            var sql = $"SELECT * FROM {defaultSchema}.Customer WHERE DeletedOn is null and CustomerId = @CustomerId";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerId = customerId });
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {defaultSchema}.Customer WHERE DeletedOn is null";
            using (var connection = this.context.CreateConnection())
            {
                return await connection.QueryAsync<Customer>(sql);
            }
        }



    }
}
