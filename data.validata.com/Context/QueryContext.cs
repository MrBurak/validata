using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace data.validata.com.Context
{
    public class QueryContext
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        public QueryContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetConnectionString("validataconnectionstring")!;
        }
        public IDbConnection CreateConnection() => new SqlConnection(this.connectionString);
    }
}
