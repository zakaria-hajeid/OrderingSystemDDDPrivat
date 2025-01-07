using EventBus.Abstraction.RabbitMq;
using EventBus.EventHandlerModel;
using EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventBus.RabbitMqImplementation
{
    public class RabbitMqConsumer<TMessage> : IRabbitMqConsumer<TMessage> where TMessage : RabbitMqEvents, new()
    {
        private readonly string queueName = new TMessage().QueueName;
        private readonly IRabbitMqConfigrationService _rabbitMqConfigrationService;
        private readonly IServiceProvider serviceProvider;
        public RabbitMqConsumer(IServiceProvider serviceProvider, IRabbitMqConfigrationService rabbitMqConfigrationService)
        {

            this.serviceProvider = serviceProvider;
            _rabbitMqConfigrationService = rabbitMqConfigrationService;
        }
        public Task Start(CancellationToken cancellationToken)
        {
            foreach(var queueDefinition in _rabbitMqConfigrationService.Build(queueName))
            {
                try
                {
                    InitializeNewConsumer(queueDefinition, _rabbitMqConfigrationService.CreateOrGetConnection(_rabbitMqConfigrationService.GetConnection("Connection"), queueDefinition.Exchange));
                }
                catch (Exception exception)
                {

                }
            }
            return Task.CompletedTask;
        }
        private void InitializeNewConsumer(QueueDefinition definition, IModel channel)
        {
            channel.ExchangeDeclare(definition.Exchange, definition.type, durable: true, autoDelete: false);
            channel.QueueDeclare(definition.Name, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(definition.Name, definition.Exchange, definition.RoutingKey);

            channel.BasicQos(0, 1, false);
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
        private async Task<bool> Handleessage(TMessage message)
        {
            var handler = serviceProvider.CreateScope().ServiceProvider.GetService<IRabbitMqEventHandler<TMessage>>();
            if (handler != null)
            {
                return await handler.HandleAsync(message);
            }
            return false;
        }
    }
}
