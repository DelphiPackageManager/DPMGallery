using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DPMGallery.DBMigration
{
    public static class MigrationRunner
    {
        public static void Execute(Assembly migrationAssembly, string connectionString, string fileName, Serilog.ILogger log)
        {

            log.Information("Initializing Database Migrations.....");
            FluentMigratorLoggerOptions options = new FluentMigratorLoggerOptions();
            // Initialize the services
            var serviceProvider = new ServiceCollection()
                .AddLogging(lb => lb.Services.AddSingleton<ILoggerProvider>(new MigrationLoggerProvider(fileName, log, options))
                .AddFluentMigratorCore()
                .AddSingleton<IConventionSet, DPMGalleryConventionSet>()
                .ConfigureRunner(builder =>
                {
                    builder = builder.AddPostgres();

                    builder = builder.WithGlobalConnectionString(connectionString)
                                     .WithMigrationsIn(migrationAssembly)
                                     .WithVersionTable(new VersionTable());

                })).BuildServiceProvider();


            // Instantiate the runner
            IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            log.Information("Running Database Migrations.....");
            // Run the migrations
            runner.MigrateUp();
            log.Information("Database Migrations Completed.");

        }
    }
}
