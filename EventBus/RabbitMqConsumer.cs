using EventBus.Abstraction;
using EventBus.EventHandlerModel;
using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace EventBus
{
    public class RabbitMqConsumer<TMessage> : IRabbitMqConsumer<TMessage> where TMessage : RabbitMqEvents, new()
    {
        private readonly string queueName = new TMessage().QueueName;
        private readonly RabbitMqOptions _options;
        protected const string DefaultConnection = "DefaultConnection";
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, IConnection> HandleBefor = new Dictionary<string, IConnection>();
        private readonly Dictionary<string, bool> ExchangeAddedBefor = new Dictionary<string, bool>();




        public RabbitMqConsumer(IOptions<RabbitMqOptions> options, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            if (this._options?.Connections == null || !this._options.Connections.ContainsKey(DefaultConnection))
                throw new Exception("Connections Must contains DefaultConnection at least");
            this.serviceProvider = serviceProvider;
        }
        public Task Start(CancellationToken cancellationToken)
        {

            var connection = CreateConnection(GetConnection(queueName + "Connection"), queueName);

            Parallel.ForEach(Build(), queueDefinition =>
            {
                try
                {
                    InitializeNewConsumer(queueDefinition, connection.CreateModel());
                }
                catch (Exception exception)
                {

                }
            });
            return Task.CompletedTask;
        }

        private IEnumerable<QueueDefinition> Build()
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
        private QueueConnectionDefinition GetConnection(string queueName)
        {
            return _options.Connections.TryGetValue(queueName, out var connection) ? connection : this._options.Connections[DefaultConnection];
        }


        private void InitializeNewConsumer(QueueDefinition definition, IModel channel)
        {
            if (ExchangeAddedBefor.TryGetValue(definition.Exchange, out bool added))
            {
                if (!added)
                {

                    channel.ExchangeDeclare(definition.Exchange, definition.type, durable: true, autoDelete: false);


                    channel.QueueDeclare(definition.Name, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    channel.QueueBind(definition.Name, definition.Exchange, definition.RoutingKey);

                    channel.BasicQos(0, 1, false);
                }
            }

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<TMessage>(Encoding.UTF8.GetString(eventArgs.Body.ToArray()));
                    if (Handleessage(message).Result)
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                    else
                        channel.BasicNack(eventArgs.DeliveryTag, false, true);

                }
                catch (Exception exception)
                {
                    channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(definition.Name, false, consumer);

        }
        private IConnection CreateConnection(QueueConnectionDefinition properties, string queueName)
        {
            if (HandleBefor.TryGetValue(queueName, out IConnection connection))
            {
                connection = HandleBefor[queueName];
                return connection;
            }
            connection = new ConnectionFactory()
            {
                HostName = properties.Host,
                VirtualHost = properties.VirtualHost,
                Port = properties.Port,
                UserName = properties.Username,
                Password = properties.Password
            }.CreateConnection();
            HandleBefor.TryAdd(queueName, connection);

            return connection;
        }
        private async Task<bool> Handleessage(TMessage message)
        {
            var handler = serviceProvider.CreateScope().ServiceProvider.GetService<IRabbitMqEventHandler<TMessage>>();
            return await handler.HandleAsync(message);
        }
    }
}
