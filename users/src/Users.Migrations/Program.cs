using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

using static System.Console;
namespace Users.Migrations
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Port=5432;Database=bookstoreuser;User Id=postgres;Password=BookStore@123;";
            long? migration = null;
            var mode = "up";

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToLower();
                switch (arg)
                {
                    case "--help":
                    case "-h":
                    case "?":
                        Usage();
                        return;
                    case "--connectionString":
                    case "-cs":
                        connectionString = args[++i];
                        break;
                    case "--migration":
                    case "-m":
                        migration = int.Parse(args[++i]);
                         break;
                    case "--mode":
                        mode = args[++i].ToLower();
                        break;
                }
            }

            migration = mode switch
            {
                "up" => migration ?? long.MaxValue,
                "down" => migration ?? long.MinValue,
                _ => 0
            };
            
            if (migration == 0)
            {
                Usage();
                return;
            }
            WriteLine($"using CS: {connectionString}");
            EnsureDatabase(connectionString);
            RunMigrations(connectionString, migration.Value, mode);
        }

        private static void RunMigrations(string connectionString, long migration, string mode)
        {
            var service = CreateServices(connectionString);
            var runner = service.GetRequiredService<IMigrationRunner>();

            WriteLine($"Running database migration {mode}");
            if (mode == "up")
            {
                runner.MigrateUp(migration);
            }
            else
            {
                runner.MigrateDown(migration);
            }
            
            WriteLine("Finish");
        }

        private static void Usage()
        {
            WriteLine("Database migration:");
            WriteLine("    --connectionString|-cs               Connection String");
            WriteLine("    --migration|-m <value>               Migration id");
            WriteLine("    --mode <value>                       Migration mode(Up | Down)");
        }


        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(AddPhoneTable).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        private static void EnsureDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var database = builder.Database;
            builder.Database = null;

            using var connection = new NpgsqlConnection(builder.ToString());
            connection.Open();
            var databasesQuery = "select * from postgres.pg_catalog.pg_database where datname = @name";

            using (var command = new NpgsqlCommand(databasesQuery, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@name", database));
                using var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    return;
                }
            }

            var createDatabaseQuery = $"CREATE DATABASE \"{database}\"";
            using var create = new NpgsqlCommand(createDatabaseQuery, connection);
            create.ExecuteNonQuery();
        }   
    }
}