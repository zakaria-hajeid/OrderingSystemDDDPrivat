using EventBus.Abstraction;
using EventBus.Events;
using EventBus.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using static MassTransit.MessageHeaders;

namespace Ordering.Application.HostedServices
{
    public class RabbitMqConsumerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqConsumerHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await AddAllConsumers(
                typeof(OrderStatusChangedToSubmittedIntegrationEvent)
                );
        }

        private Task AddAllConsumers(params Type[] messageAssemblyMarkerTypes)
        {
            var assembliesToScan = messageAssemblyMarkerTypes.Select((Type t) => t.GetTypeInfo().Assembly);

            var messageTypes = assembliesToScan
               .SelectMany(a => a.GetExportedTypes())
               .Where(type => !type.IsAbstract && typeof(RabbitMqEvents).IsAssignableFrom(type));

            IServiceScope scope = _serviceScopeFactory.CreateScope();

            foreach (var type in messageTypes)
            {
                var consumer = scope.ServiceProvider.GetRequiredService(typeof(IRabbitMqConsumer<>).MakeGenericType(type));
                consumer.GetType().GetMethod("Start")?.Invoke(consumer, new object[1] { new CancellationToken() });
            }
            return Task.CompletedTask;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;

        }
    }
}
