using EventBus;
using EventBus.Abstraction;
using IntegrationEventLogEF.DbContexts;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using System.Linq;

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

        public static WebApplication UseServiceDefaults(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = app.Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
                app.UseRouting();

                var identitySection = app.Configuration.GetSection("Identity");

                if (identitySection.Exists())
                {
                    // We have to add the auth middleware to the pipeline here
                    app.UseAuthentication();
                    app.UseAuthorization();
                }
            }

            //app.UseDefaultOpenApi(app.Configuration);

            app.MapDefaultHealthChecks();

            return app;
        }

        public static void MapDefaultHealthChecks(this IEndpointRouteBuilder routes)
        {
           /* routes.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = "Helthy"
            });*/

            routes.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        }
        public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultHealthChecks(builder.Configuration);
            return builder;
        }
            public static IHealthChecksBuilder AddDefaultHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var eventBusSection = configuration.GetSection("EventBusMessageBroker");

            if (!eventBusSection.Exists())
            {
                return hcBuilder;
            }


            return hcBuilder;/*hcBuilder.AddRabbitMQ(
                         $"amqp://{configuration.GetRequiredConnectionString("EventBusMessageBroker")}",
                        name: "rabbitmq");*/


        }
        public static WebApplication MapSpeacificHelthCheck(this WebApplication app, List<KeyValuePair<string, string>> hcecksRoutWithName)
        {
            foreach (var hc in hcecksRoutWithName)
            {
                app.MapHealthChecks($"/{hc.Key}", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains(hc.Value)
                });
            }

            return app;
        }


    }

}
