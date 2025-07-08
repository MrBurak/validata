using data.validata.com.Context;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;

namespace data.validata.test.Context
{
    public class QueryContextTests
    {
        [Fact]
        public void Constructor_InitializesConnectionStringCorrectly()
        {
            var expectedConnectionString = "Server=myServer;Database=myDataBase;Integrated Security=True;";
            var mockConfiguration = new Mock<IConfiguration>();
            var mockConfigurationSection = new Mock<IConfigurationSection>();

            mockConfiguration.Setup(c => c.GetSection("ConnectionStrings"))
                             .Returns(mockConfigurationSection.Object);
            mockConfigurationSection.Setup(cs => cs["validataconnectionstring"])
                                    .Returns(expectedConnectionString);
            var context = new QueryContext(mockConfiguration.Object);

 
            var connectionStringField = typeof(QueryContext)
                                        .GetField("connectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                                        .GetValue(context) as string;

            Assert.Equal(expectedConnectionString, connectionStringField);
        }

        [Fact]
        public void CreateConnection_ReturnsSqlConnectionWithCorrectConnectionString()
        {
            var expectedConnectionString = "Data Source=.;Initial Catalog=TestDb;Integrated Security=True;";
            var mockConfiguration = new Mock<IConfiguration>();
            var mockConfigurationSection = new Mock<IConfigurationSection>();

            mockConfiguration.Setup(c => c.GetSection("ConnectionStrings"))
                             .Returns(mockConfigurationSection.Object);
            mockConfigurationSection.Setup(cs => cs["validataconnectionstring"])
                                    .Returns(expectedConnectionString);

            var context = new QueryContext(mockConfiguration.Object);

            using var connection = context.CreateConnection();

            Assert.NotNull(connection);
            Assert.IsType<SqlConnection>(connection);
            Assert.Equal(expectedConnectionString, ((SqlConnection)connection).ConnectionString);
        }

        
    }
}
