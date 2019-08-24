using IdentityServer4.Storage.DatabaseScripts.DbUp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace AspNetCore.Identity.DatabaseScripts.DbUp
{
    /// <summary>
    /// Extension methods to add Dapper support to IdentityServer.
    /// </summary>
    public static class IdentityServerDbScriptsExtensions
    {


        public static IServiceCollection AddIdentityDbUpDatabaseScripts(this IServiceCollection services, Action<DBProviderOptions> dbProviderOptionsAction = null)
        {
            var options = GetDefaultOptions();
            dbProviderOptionsAction?.Invoke(options);
            services.AddSingleton(options);
            services.TryAddTransient<IIdentityMigrations, Migrations>();
            return services;
        }
        public static DBProviderOptions GetDefaultOptions()
        {
            //config mssql
            var options = new DBProviderOptions();
            return options;
        }



    }
}
