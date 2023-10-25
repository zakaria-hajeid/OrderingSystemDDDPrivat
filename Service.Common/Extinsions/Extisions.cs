using EventBus;
using EventBus.Abstraction;
using IntegrationEventLogEF.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Common.Extinsions
{
    public static class Extisions
    {
        public static IServiceCollection AddSharingDbContexts(this IServiceCollection services, IConfiguration configuration, string assymblyName, string connectionStringSectionName)

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
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration, bool withConsumer = false,
            Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>> queeWithConsumer = null)

        {
            services.AddScoped<IEventBus, EventBusHandler>();
            
            if (withConsumer)
            {
                AddEventBusWithConsumer(services, configuration, queeWithConsumer);

            }
            else
            {
                AddEventBus(services, configuration);
            }
            services.AddMassTransitHostedService();
            return services;
        }
        private static void AddEventBus(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(bus =>
            {


                bus.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["EventBusMessageBroker:host"], Convert.ToUInt16(configuration["EventBusMessageBroker:Port"]), "/",
                                        h =>
                                        {
                                            h.Username(configuration["EventBusMessageBroker:UserName"]);
                                            h.Password(configuration["EventBusMessageBroker:Password"]);
                                        });
                    cfg.ConfigureEndpoints(context);
                
                });
            });

        }

        private static void AddEventBusWithConsumer(IServiceCollection services, IConfiguration configuration,
                   Dictionary<string, List<Action<IRabbitMqReceiveEndpointConfigurator, IBusRegistrationContext>>> queeWithConsumer = null)

        {

            services.AddMassTransit(bus =>
                        {

                            bus.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.Host(configuration["EventBusMessageBroker:host"], Convert.ToUInt16(configuration["EventBusMessageBroker:Port"]), "/",
                                                    h =>
                                                    {
                                                        h.Username(configuration["EventBusMessageBroker:UserName"]);
                                                        h.Password(configuration["EventBusMessageBroker:Password"]);
                                                    });
                                cfg.ConfigureEndpoints(context);

                                //Declar Quee And BindEachConsumer
                                foreach (var consumer in queeWithConsumer)
                                {
                                    cfg.ReceiveEndpoint(consumer.Key, e => //QueueDeclareOk queue with consumer and bind into defaul or specific exchang type 
                                    {
                                        consumer.Value.ForEach(s => s.Invoke(e, context));
                                    });
                                }

                            });

                        });
        }
    }
}
