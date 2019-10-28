using DbUp;
using DbUp.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.Identity.DatabaseScripts.DbUp
{
    public class Migrations : IIdentityMigrations
    {
        private readonly string _connectionString;
        private readonly string _schema;

        public Migrations(DBProviderOptions options)
        {

            _schema = options.DbSchema;
            _connectionString = options.ConnectionString;

        }

        public Migrations(string connectionString, string schema)
        {

            _schema = schema;
            _connectionString = connectionString;

        }


        public bool UpgradeDatabase()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Trying to upgrade database");
            Console.ResetColor();
            EnsureDatabase.For.SqlDatabase(_connectionString);
            var fullSuccess = true;
            var result = UpgradeDatabase(_connectionString, _schema, "DbScripts");
            if (result == -1)
                fullSuccess = false;

            return fullSuccess;
        }

        public static int EnsureSchema(string connectionString, string schema)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Preparing to upgrade {schema}");
            var variableSubstitutions = new Dictionary<string, string>();
            variableSubstitutions.Add("schemaname", $"{schema}");

            Console.ResetColor();
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (s) => s.Contains("EveryRun"))
                .WithVariable("schemaname", $"{schema}")
                .JournalTo(new NullJournal())
                .WithTransaction()
                .LogToConsole();

            var upgrader = upgradeEngine.Build();
            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
#if DEBUG
                    Console.ReadLine();
#endif
                    return -1;
                }
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }


        public static int UpgradeDatabase(string connectionString, string schema, string scriptFolder)
        {
            EnsureSchema(connectionString, schema);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Preparing to upgrade {schema}");
            var variableSubstitutions = new Dictionary<string, string>();
            variableSubstitutions.Add("schemaname", $"{schema}");

            Console.ResetColor();
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (s) => s.Contains(scriptFolder))
                .WithVariable("schemaname", $"{schema}")
                .JournalToSqlTable(schema, "SchemaVersions")
                .WithTransaction()
                .LogToConsole();

            var upgrader = upgradeEngine.Build();
            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
#if DEBUG
                    Console.ReadLine();
#endif
                    return -1;
                }
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();


            return 0;


        }

    }
}