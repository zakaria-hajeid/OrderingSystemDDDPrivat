using EventBus.Events;

namespace EventBus.Abstraction
{
    public interface IRabbitMqPublisher<TMessage> where TMessage : RabbitMqEvents
    {
        void Publish(TMessage message);

    }
}
