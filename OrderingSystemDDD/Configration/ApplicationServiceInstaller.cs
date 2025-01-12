using EventBus;
using EventBus.Abstraction.RabbitMq;
using EventBus.EventHandlerModel;
using EventBus.Events;
using EventBus.IntegrationEvents;
using FluentValidation;
using MediatR.NotificationPublishers;
using Ordering.Application;
using Ordering.Application.Behaviors;
using Service.Common.Extinsions;
using AssemblyReference = Ordering.Application.AssemblyReference;

namespace OrderingSystemDDD.Configration
{
    public class ApplicationServiceInstalle : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(AssemblyReference));
                //The area the use the request and request handler
                //cfg.NotificationPublisher = new ForeachAwaitPublisher();each notification handler execute one by one
                cfg.NotificationPublisher = new TaskWhenAllPublisher(); //each notification handler execute in parallel way
                                                                        //Note :The fist pipline add is first invoke sequentially 
                cfg.AddOpenBehavior(typeof(IdempotencyCommandBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            services.AddValidatorsFromAssembly(EventBus.AssemblyReference.assembly);

            services.AddEventBusSharedServices(configuration);
            services.Configure<RabbitMqOptions>(options => configuration.GetSection(nameof(RabbitMqOptions)).Bind(options));
            services.AddRabbitMqPublishers();
            services.AddRabbitMqConfigration();


        }
    }
}
