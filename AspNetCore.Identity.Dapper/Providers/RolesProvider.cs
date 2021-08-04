using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Dapper.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Dapper.Providers
{
    internal class RolesProvider
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RolesProvider(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            var command = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);";

            int rowsInserted;

            await using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsInserted = await sqlConnection.ExecuteAsync(command, new {
                    role.Id,
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp
                });
            }

            return rowsInserted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be inserted."
            });
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role) {
            var command = $"UPDATE [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "SET Name = @Name, NormalizedName = @NormalizedName, ConcurrencyStamp = @ConcurrencyStamp " +
                                   "WHERE Id = @Id;";

            await using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                await using var transaction = await sqlConnection.BeginTransactionAsync();
                await sqlConnection.ExecuteAsync(command, new {
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp,
                    role.Id
                }, transaction);

                if (role.Claims.Count > 0) {
                     var deleteClaimsCommand = "DELETE " +
                                                       $"FROM [{_databaseConnectionFactory.DbSchema}].RoleClaims " +
                                                       "WHERE RoleId = @RoleId;";

                    await sqlConnection.ExecuteAsync(deleteClaimsCommand, new {
                        RoleId = role.Id
                    }, transaction);

                    var insertClaimsCommand = $"INSERT INTO [{_databaseConnectionFactory.DbSchema}].RoleClaims (RoleId, ClaimType, ClaimValue) " +
                                                 "VALUES (RoleId, ClaimType, ClaimValue);";

                    await sqlConnection.ExecuteAsync(insertClaimsCommand, role.Claims.Select(x => new {
                        RoleId = role.Id,
                        ClaimType = x.Type,
                        ClaimValue = x.Value
                    }), transaction);
                }

                try {
                    await transaction.CommitAsync();
                } catch {
                    try {
                        await transaction.RollbackAsync();
                    } catch {
                        return IdentityResult.Failed(new IdentityError {
                            Code = nameof(UpdateAsync),
                            Description = $"Role with name {role.Name} could not be updated. Operation could not be rolled back."
                        });
                    }

                    return IdentityResult.Failed(new IdentityError {
                        Code = nameof(UpdateAsync),
                        Description = $"Role with name {role.Name} could not be updated.. Operation was rolled back."
                    });
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role) {
            var command = "DELETE " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE Id = @Id;";

            int rowsDeleted;

            await using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync()) {
                rowsDeleted = await sqlConnection.ExecuteAsync(command, new { role.Id });
            }

            return rowsDeleted == 1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be deleted."
            });
        }

        public async Task<ApplicationRole> FindByIdAsync(Guid roleId) {
             var command = "SELECT * " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE Id = @Id;";

             await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
             return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                 Id = roleId
             });
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName) {
             var command = "SELECT * " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles " +
                                   "WHERE NormalizedName = @NormalizedName;";

             await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
             return await sqlConnection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new {
                 NormalizedName = normalizedRoleName
             });
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync() {
            var command = "SELECT * " +
                                   $"FROM [{_databaseConnectionFactory.DbSchema}].AspNetRoles;";

            await using var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync();
            return await sqlConnection.QueryAsync<ApplicationRole>(command);
        }
    }
}
