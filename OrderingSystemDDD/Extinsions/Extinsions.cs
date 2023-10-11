using IntegrationEventLogEF.DbContexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ordering.Persistence;
using Service.Common.Extinsions;

namespace OrderingSystemDDD.Extinsions
{
    public static class Extinsions
    {
        public static IServiceCollection AddDbContextsExtinstions(this IServiceCollection services, IConfiguration configuration)
        {
            static void ConfigureSqlOptions(SqlServerDbContextOptionsBuilder sqlOptions)
            {

                sqlOptions.MigrationsAssembly("OrderingSystemDDD");
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay:
                            TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            };
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetRequiredConnectionString("OrderingDB"), ConfigureSqlOptions);
            });
            services.AddSharingDbContexts(configuration, "OrderingSystemDDD", "OrderingDB");
            /*services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration.GetRequiredConnectionString("OrderingDB"), ConfigureSqlOptions);
            });*/
            return services;
        }

    }
}
