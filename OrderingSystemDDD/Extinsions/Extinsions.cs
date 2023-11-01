using IntegrationEventLogEF.DbContexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ordering.Persistence;
using Service.Common.Extinsions;
using Ordering.Domain.Sahred;
using Microsoft.AspNetCore.Server.IIS.Core;

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

        public static T Match<T>(this Result result, Func<T> onSuccess, Func<Error[], T> onFailure)
        {
            if(result is IValidationResult)
            {
                IValidationResult validationResult  = (IValidationResult)result;
                return onFailure(validationResult.Errors);
            }
            Error[] errors = new Error[]
            {
                result.Error
            };
            return result.IsSuccess ? onSuccess() : onFailure(errors);
        }


    }
}
