using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    public interface IDatabaseConnectionFactory
    {
        Task<SqlConnection> CreateConnectionAsync();
        string DbSchema { get; set; }
    }
}
