using IntegrationEventLogEF.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Extinsions
{
    public static class Extisions
    {
        public static IServiceCollection AddSharingDbContexts(this IServiceCollection services, IConfiguration configuration, string assymblyName , string connectionStringSectionName)

        {
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                var connectionString = configuration.GetRequiredConnectionString(connectionStringSectionName);
                options.UseSqlServer(connectionString, option =>
                {
                    option.MigrationsAssembly(assymblyName);
                    option.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }
                );
            });
            return services;
        }
    }
}
