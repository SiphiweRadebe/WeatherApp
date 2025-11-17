using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp.Migrations
{
    public static class MigrationRunner
    {
        public static void MigrateDatabase(string connectionString)
        {
            // First, ensure the database exists
            EnsureDatabaseExists(connectionString);

            // Then run migrations
            var serviceProvider = CreateServices(connectionString);

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            // Connect to master database to create our database
            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                var checkDbCommand = connection.CreateCommand();
                checkDbCommand.CommandText = $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'";
                var dbExists = checkDbCommand.ExecuteScalar();

                if (dbExists == null)
                {
                    var createDbCommand = connection.CreateCommand();
                    createDbCommand.CommandText = $"CREATE DATABASE [{databaseName}]";
                    createDbCommand.ExecuteNonQuery();

                    Console.WriteLine($"Database '{databaseName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"Database '{databaseName}' already exists.");
                }
            }
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
