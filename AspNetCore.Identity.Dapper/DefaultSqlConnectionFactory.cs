using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    public class DefaultSqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public DefaultSqlConnectionFactory(string connectionString, string schema)
        {
            _connectionString = connectionString ?? string.Empty;
            DbSchema = schema;
        }

        public async Task<SqlConnection> CreateConnectionAsync() {
            try {
                var sqlConnection = new SqlConnection(_connectionString);
                await sqlConnection.OpenAsync();
                return sqlConnection;
            } catch {
                throw;
            }
        }

        public string DbSchema { get; set; }
    }
}
