using EventBus.Events;

namespace EventBus.Abstraction
{
    public interface IRabbitMqConsumer<TMessage> where TMessage : RabbitMqEvents
    {
        Task Start(CancellationToken cancellationToken);

    }
}
