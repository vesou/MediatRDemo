using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NormalApi.DAL;

namespace NormalApi
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await CreateDbIfNotExists(host);
            await host.RunAsync();
        }

        private static async Task CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApiContext>();
                await DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB");
            }
        }
    }
}