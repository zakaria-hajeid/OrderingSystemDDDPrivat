using EventBus.Events;

namespace EventBus.Abstraction.RabbitMq
{
    public interface IRabbitMqPublisher<TMessage> where TMessage : RabbitMqEvents
    {
        void Publish(TMessage message);

    }
}
