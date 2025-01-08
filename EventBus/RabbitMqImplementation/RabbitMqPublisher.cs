using EventBus.Abstraction.RabbitMq;
using EventBus.EventHandlerModel;
using EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace EventBus.RabbitMqImplementation
{
    public class RabbitMqPublisher<TMessage> : IRabbitMqPublisher<TMessage> where TMessage : RabbitMqEvents, new()
    {
        private readonly string queueName = new TMessage().QueueName;
        protected const string DefaultConnection = "DefaultConnection";
        private readonly IRabbitMqConfigrationService _rabbitMqConfigrationService;

        public RabbitMqPublisher(IRabbitMqConfigrationService rabbitMqConfigrationService)
        {
            _rabbitMqConfigrationService = rabbitMqConfigrationService;
        }
        void IRabbitMqPublisher<TMessage>.Publish(TMessage message)
        {
            IModel channel = null;
            QueueDefinition definition = new();

            string selectedRoutingKey = null;
            uint minMessageCount = int.MaxValue;

            foreach (var queueDefinition in _rabbitMqConfigrationService.Build(queueName))
            {
                channel = _rabbitMqConfigrationService.CreateOrGetConnection(_rabbitMqConfigrationService.GetConnection("Connection"), queueDefinition.Exchange);

                //get the queue that have least no of message to puplish into it when one queue have multiple channels
                var queueDeclareOk = channel.QueueDeclarePassive(queueDefinition.Name);

                if (queueDeclareOk == null)
                {
                    definition = queueDefinition;
                    break;
                }
                uint messageCount = queueDeclareOk.MessageCount;

                if (messageCount < minMessageCount)
                {
                    minMessageCount = messageCount;
                    definition = queueDefinition;
                }
            }
            channel.ExchangeDeclare(definition.Exchange, definition.type, durable: true, autoDelete: false);
            channel.QueueDeclare(definition.Name, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(definition.Name, definition.Exchange, definition.RoutingKey);

            channel.BasicPublish(
              definition.Exchange,
              definition.RoutingKey,
              channel.CreateBasicProperties(),
              Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));

        }

    }
}
