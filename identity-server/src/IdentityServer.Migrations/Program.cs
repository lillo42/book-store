using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Postgres;
using IdentityServer.Infrastructure;
using IdentityServer.Infrastructure.Abstractions;
using IdentityServer.Migrations.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Raven.Client.Documents;
using Serilog;
using Serilog.Events;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using static System.Console;

namespace IdentityServer.Migrations
{
    public class Program
    {
        private static ILogger _logger;
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()    
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Execute(args);
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Migration terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Execute(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var provider = CreateServices(configuration);
            
            _logger = provider.GetRequiredService<ILogger<Program>>();
            
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
            
            EnsureDatabase(configuration.GetConnectionString("Postgres"));
            RunMigrations(migration.Value, mode, provider);
        }
        
        private static void Usage()
        {
            WriteLine("Database migration:");
            WriteLine("    --connectionString|-cs               Connection String");
            WriteLine("    --migration|-m <value>               Migration id");
            WriteLine("    --mode <value>                       Migration mode(Up | Down)");
        }


        private static void RunMigrations(long migration, string mode, IServiceProvider service)
        {
            var runner = service.GetRequiredService<IMigrationRunner>();

            _logger.LogInformation("Running database migration {mode}", mode);
            if (mode == "up")
            {
                runner.MigrateUp(migration);
            }
            else
            {
                runner.MigrateDown(migration);
            }
            
            _logger.LogInformation("Finish");
        }
        
        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static IServiceProvider CreateServices(IConfiguration configuration)
        {
            var service = new ServiceCollection()
                .AddSingleton(configuration)
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .AddLogging(builder => builder.AddSerilog())
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(provider =>
                        provider.GetRequiredService<IConfiguration>().GetConnectionString("Postgres"))
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(AddClient).Assembly).For.Migrations());
            
            service.AddScoped<PostgresQuoter, CustomQuote>();
            service.AddSingleton<IDocumentStore>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>()
                    .GetSection("ConnectionString")
                    .GetSection("RavenDb");
                return new DocumentStore
                {
                    Urls = configuration.GetValue<IEnumerable<string>>("Url").ToArray(),
                    Database = configuration.GetValue<string>("Database")
                };
            });

            var container = new ContainerBuilder();
            container.Populate(service);
            
            container.RegisterType<SHA256Algorithm>()
                .As<IHashAlgorithm>()
                .SingleInstance();
            
            return new AutofacServiceProvider(container.Build());
        }
        
        private static void EnsureDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var database = builder.Database;
            builder.Database = null;

            _logger.LogInformation("Going to ensure '{database}' database exist.", database);
            
            using var connection = new NpgsqlConnection(builder.ToString());
            connection.Open();
            const string databasesQuery = "select * from postgres.pg_catalog.pg_database where datname = @name";

            using (var command = new NpgsqlCommand(databasesQuery, connection))
            {
                command.Parameters.Add(new NpgsqlParameter("@name", database));
                using var result = command.ExecuteReader();
                if (result.HasRows)
                {
                    _logger.LogInformation("'{database}' database exist.", database);
                    return;
                }
            }

            _logger.LogInformation("'{database}' database don't exist going to create.", database);
            var createDatabaseQuery = $"CREATE DATABASE \"{database}\"";
            using var create = new NpgsqlCommand(createDatabaseQuery, connection);
            create.ExecuteNonQuery();
        }
    }
}