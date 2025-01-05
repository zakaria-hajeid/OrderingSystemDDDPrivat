using EventBus.EventHandlerModel;
using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EventBus.Abstraction
{
    public class RabbitMqPublisher<TMessage> : IRabbitMqPublisher<TMessage> where TMessage : RabbitMqEvents, new()
    {
        private readonly string queueName = new TMessage().QueueName;
        protected const string DefaultConnection = "DefaultConnection";
        private readonly RabbitMqOptions _options;
        private readonly Dictionary<string, IModel> connections = new Dictionary<string, IModel>();

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }
        void IRabbitMqPublisher<TMessage>.Publish(TMessage message)
        {

            IModel channel = null;
            var definition = Build(message);
            if (connections.TryGetValue(definition.Name, out var connection))
            {
                channel = connections[definition.Name];
            }
            else
            {
                channel = CreateConnection(GetConnection(queueName +"Connection")).CreateModel();
            }

            channel.ExchangeDeclare(definition.Exchange, definition.type, durable: true, autoDelete: false);
            channel.QueueDeclare(definition.Name, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(definition.Name, definition.Exchange, definition.RoutingKey);

            channel.BasicPublish(
              definition.Exchange,
              definition.RoutingKey,
              channel.CreateBasicProperties(),
              Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
            connections.TryAdd(definition.Name, channel);

        }
        private QueueDefinition Build(TMessage message)
        {
            var consumer = GetConsumerByQueueName(queueName);
            return new QueueDefinition
            {
                Name = $"{queueName}.{message.Channel}",
                RoutingKey = $"{queueName}.{message.Channel}.*",
                Exchange = consumer.Exchange,
                type = consumer.type
            };
        }
        private ConsumerDefinition GetConsumerByQueueName(string queueName)
        {
            return _options.Consumers.TryGetValue(queueName, out var consumer) ? consumer : new ConsumerDefinition();
        }
        private QueueConnectionDefinition GetConnection(string queueName)
        {
            return _options.Connections.TryGetValue(queueName, out var connection) ? connection : this._options.Connections[DefaultConnection];
        }
        private IConnection CreateConnection(QueueConnectionDefinition properties)
        {
            var connection = new ConnectionFactory()
            {
                HostName = properties.Host,
                VirtualHost = properties.VirtualHost,
                Port = properties.Port,
                UserName = properties.Username,
                Password = properties.Password
            }.CreateConnection();

            return connection;
        }
        private IModel GetOrCreateConnection(QueueDefinition definition, IModel channel)
        {



            return channel;
        }


    }
}
