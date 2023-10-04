using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Persistence;
using Ordering.Persistence.Repository;

namespace OrderingSystemDDD.Configration
{
    public class InftastructiorServiceInstaller : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetRequiredConnectionString("OrderingDB"), x =>
                {
                    x.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay:
                        TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    x.MigrationsAssembly("OrderingSystemDDD");

        });
            });
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
