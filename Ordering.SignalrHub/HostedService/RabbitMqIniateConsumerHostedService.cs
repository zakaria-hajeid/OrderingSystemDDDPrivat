using EventBus.Abstraction.RabbitMq;
using EventBus.IntegrationEvents;

namespace Ordering.SignalrHub.HostedService
{
    public class RabbitMqIniateConsumerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMqIniateConsumerHostedService(IServiceScopeFactory serviceScopeFactory)
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
            /* var assembliesToScan = messageAssemblyMarkerTypes.Select((Type t) => t.GetTypeInfo().Assembly);

             var messageTypes = assembliesToScan
                .SelectMany(a => a.GetExportedTypes())
                .Where(type => !type.IsAbstract && typeof(RabbitMqEvents).IsAssignableFrom(type));*/

            IServiceScope scope = _serviceScopeFactory.CreateScope();

            foreach (var type in messageAssemblyMarkerTypes)
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
