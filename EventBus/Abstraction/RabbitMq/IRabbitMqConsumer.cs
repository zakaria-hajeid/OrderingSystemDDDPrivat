using EventBus.Events;

namespace EventBus.Abstraction.RabbitMq
{
    public interface IRabbitMqConsumer<TMessage> where TMessage : RabbitMqEvents
    {
        Task Start(CancellationToken cancellationToken);

    }
}
