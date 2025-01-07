using EventBus.Abstraction.RabbitMq;
using EventBus.IntegrationEvents;
using MassTransit;
using Ordering.SignalrHub.RabbitMqHandlers.EvenstHandlers;

namespace Ordering.SignalrHub.Extinsions
{
    public static class RabbitMqExtinsions
    {
        public static void BusRegistrationConfigurator(this IBusRegistrationConfigurator BusRegistration )
        {
            
        }
        public static IServiceCollection AddOrderStatusChangeToSubmittedHandlerEventExtensions(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>, OrderStatusChangeToSubmittedHandler>();
            return services;
        }
    }
}
