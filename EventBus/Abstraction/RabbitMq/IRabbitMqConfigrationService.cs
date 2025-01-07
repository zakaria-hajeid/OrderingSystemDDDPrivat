using EventBus.EventHandlerModel;
using EventBus.Events;
using RabbitMQ.Client;

namespace EventBus.Abstraction.RabbitMq
{
    public interface IRabbitMqConfigrationService
    {
        IEnumerable<QueueDefinition> Build(string queueName);
        IModel CreateOrGetConnection(QueueConnectionDefinition properties, string exchangNameToSaveConnection="");
        QueueConnectionDefinition GetConnection(string connectionName);
    }
}
