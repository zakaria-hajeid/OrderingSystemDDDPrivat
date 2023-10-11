using IntegrationEventLogEF.Repository;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Infrastructure.Services;
using Ordering.Persistence;
using Ordering.Persistence.Repository;
using OrderingSystemDDD.Extinsions;
using System.Data.Common;

namespace OrderingSystemDDD.Configration
{
    public class InftastructiorServiceInstaller : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextsExtinstions(configuration);
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddTransient<Func<DbConnection, IntegrationEventLogService>>(
                                sp => (DbConnection c) => new IntegrationEventLogService(
                                                               () => new IIntegrationOutboxRepository(c)));

            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

        }


    }
}
