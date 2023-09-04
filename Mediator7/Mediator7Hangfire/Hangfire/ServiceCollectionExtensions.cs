using Hangfire;
using Hangfire.MySql;
using Mediator7Hangfire.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Mediator7Hangfire.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireServ(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            ConnectionHelper.CreateMySqlConnectionString(
                configuration.GetOptions<DatabaseConnectionOptions>("Databases:Hangfire"));

        // Configure hangfire properties
        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(
                    new MySqlStorage(
                        connectionString,
                        new MySqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(10),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 25000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablesPrefix = "Hangfire"
                        }
                    )
                );
            config.UseMediatR();
        });

        services.AddHangfireServer(options =>
        {
            List<string> queues = new List<string> { "default" };
            
            options.Queues = queues.ToArray();
        });

        return services;
    }
}