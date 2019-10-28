using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper.Models;
using AspNetCore.Identity.Dapper.Stores;
using Dapper;

namespace AspNetCore.Identity.Dapper.Providers
{
    internal class UserRolesProvider
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserRolesProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IEnumerable<UserRole>> GetRolesAsync(ApplicationUser user) {
            var command = "SELECT r.Id AS RoleId, r.Name AS RoleName " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles AS r " +
                                   $"INNER JOIN [{_databaseConnectionFactory.DbSchema}].[AspNetUserRoles] AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            return await sqlConnection.QueryAsync<UserRole>(command, new {
                UserId = user.Id
            });
        }
    }
}
