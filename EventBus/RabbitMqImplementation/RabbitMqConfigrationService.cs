using EventBus.Abstraction.RabbitMq;
using EventBus.EventHandlerModel;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EventBus.RabbitMqImplementation
{
    public class RabbitMqConfigrationService : IRabbitMqConfigrationService
    {
        private readonly Dictionary<string, IModel> connections = new Dictionary<string, IModel>();
        private readonly RabbitMqOptions _options;

        public RabbitMqConfigrationService(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            if (_options?.Connections == null)
                throw new Exception("Connections Must contains DefaultConnection at least");
        }
        public IEnumerable<QueueDefinition> Build(string queueName)
        {
            var consumer = GetConsumerByQueueName(queueName);
            if (!consumer.Active) yield break;
            foreach (var channel in consumer.Channels)
            {
                yield return new QueueDefinition
                {
                    Name = $"{queueName}.{channel}",
                    RoutingKey = $"{queueName}.{channel}.*",
                    Exchange = consumer.Exchange,
                    type = consumer.type
                };
            }
        }
        private ConsumerDefinition GetConsumerByQueueName(string queueName)
        {
            return _options.Consumers.TryGetValue(queueName, out var consumer) ? consumer : new ConsumerDefinition();
        }
        public QueueConnectionDefinition GetConnection(string connectionName)
        {
            return _options.Connections.TryGetValue(connectionName, out var connection) ? connection : new QueueConnectionDefinition();
        }
        public IModel CreateOrGetConnection(QueueConnectionDefinition properties, string exchangNameToSaveConnection = "")
        {
            var connection = new ConnectionFactory()
            {
                HostName = properties.Host,
                VirtualHost = properties.VirtualHost,
                Port = properties.Port,
                UserName = properties.Username,
                Password = properties.Password
            };
            if (!string.IsNullOrEmpty(exchangNameToSaveConnection))
            {
                if (connections.TryGetValue(exchangNameToSaveConnection, out var channel))
                {
                    return channel;
                }
                IModel newChannel = connection.CreateConnection().CreateModel();
                connections.TryAdd(exchangNameToSaveConnection, newChannel);
            }
            return connection.CreateConnection().CreateModel();
        }


    }
}
