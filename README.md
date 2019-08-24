# AspNetCore.Identity.Dapper
This project provides a Dapper implementation of UserStore and RoleStore, to be used with Microsoft.AspNetCore.Identity.

## Background
This project came about after looking around at other Dapper implementations of AspNetCore.Identity but struggling to find something that was updated with the latest Dotnet Core version. This implementation is made for Dotnet Core and if used with anything else would have to be rewritten - atleast partially.

I needed this to be run together with IdentityServer4, on a schema I specified. It works completely transparant for IdentityServer4, just make sure to use the ApplicationUser entity when adding it to the AddIdentityServer() middleware as shown below.

```

    //Code Omitted

    var builder = services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.IssuerUri = authority;
        })
        .AddAspNetIdentity<ApplicationUser>();

    //Code Omitted

```

## Main features

The library provides an implementation of the UserStore and RoleStore used by ASP.NET Core Identity. It is an alternative to using the EntityFramework implementation (middleware: .AddEntityFrameworkStores<ApplicationDbContext>()), if I remember correctly from the package Microsoft.AspNetCore.Identity.EntityFrameworkCore.

More documentation on AspNetCore.Identity can be found here:

[Getting started with ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio)

[Configure ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-2.2)


If you are just beginning out with IdentityServer4, it is important to note that this library will help you with persistence of the IdentityServer4 settings (such as which Clients to authorize, which ApiResources and such) but anything related to **Users** is not handled in this library. 

Both libraries are made to let the developer customize ConnectionString and DatabaseSchema.
It is built for Dotnet Core (latest version as of august 2019: 2.2.6), with DependencyInjection, making customization easy.


## Getting Started

These instructions will help you get started.

There is two packages currently.

**IdentityServer4.Dapper.Storage**: This package is the library itself that replaces the stores Microsoft.AspNetCore.Identity uses.

**IdentityServer4.Storage.DatabaseScripts.DbUp**: This is a small project which contains SQL scripts to create databases. It uses DbUp, but you can just take the SQL scripts and use them with any database migrations you might use. 


### Prerequisites

You will need Microsoft.AspNetCore.Identity in order to use this library.

```
dotnet add package VeryGood.AspNetCore.Identity.Dapper
dotnet add package VeryGood.AspNetCore.Identity.DatabaseScripts.DbUp
```

### Get AspNetCore.Identity.Dapper working

In order to enable the AspNetCore.Identity.Dapper, invoke the middleware following the standard .AddIdentityCore or AddIdentity in ConfigureServices.
Make sure when specifying schema, to not include brackets.

**Example with AddIdentityCore**

```
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            Configuration omitted
            */

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
            })
            .AddRoles<ApplicationRole>()
            .AddDapperStores(options =>
            {
                options.ConnectionString = "my connectionString";
                options.DbSchema = "my schema";
            })
            .AddDefaultTokenProviders();

            
            /* 
            Configuration omitted
             */

        }
```

It is important to add ApplicationUser and ApplicationRole as the standard User/Role entities.

Another example is provided below with usage of .AddIdentity().

```
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            Configuration omitted
            */
            
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                })
                .AddDapperStores(options =>
                {
                    options.ConnectionString = "my connectionString";
                    options.DbSchema = "my schema";
                })
                .AddDefaultTokenProviders();


            /* 
            Configuration omitted 
            */

        }
```

When creating new users or otherwise interacting with AspNetCore.Identity, it should be done through the UserManager / RoleManager as you normally would (if you were using EntityFramework for example).


### Get AspNetCore.Identity.DatabaseScripts.DbUp working

This library is quite simple. It has in the IIdentityServerMigrations interface one function, UpgradeDatabase

```
bool UpgradeDatabase();
```

The library itself does two things;
1. Ensures the Schema exists, and if not creates it.
2. Creates the tables needed by AspNetCore.Identity.

In order to enable the AspNetCore.Identity.DatabaseScripts.DbUp, just Add the middleware in ConfigureServices. This is done with AddIdentityDbUpDatabaseScripts.
The schema should be specified **without* brackets, and if not specified will default to dbo.

```
    public void ConfigureServices(IServiceCollection services)
        {
            /*
            Configuration omitted
            */
            var connectionString = "insert your own connection string";
            services.AddIdentityDbUpDatabaseScripts(options => {
                        options.ConnectionString = connectionString;
                        options.DbSchema = "my schema";
                        });
       
            /* 
            Configuration omitted 
            */

        }
```

Once this step has been done, it will use the Schema and ConnectionString provided once you call the UpgradeDatabase() function on the IIdentityServerMigrations interface.

It is also possible to create the Migrations class directly and use it without the middleware. This would be done like in the example below

```
            /* code omitted */

            var connectionString = "my connectionstring";
            var schema = "my schema";
            var identityMigrations = new AspNetCore.Identity.DatabaseScripts.DbUp.Migrations(connectionString, schema);
            var identityResult = identityMigrations.UpgradeDatabase();

            /* code omitted */
```