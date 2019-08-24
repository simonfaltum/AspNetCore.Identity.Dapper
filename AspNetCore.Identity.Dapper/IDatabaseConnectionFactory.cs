using System.Data;
using System.Threading.Tasks;

namespace AspNetCore.Identity.Dapper
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
        string DbSchema { get; set; }
    }
}
