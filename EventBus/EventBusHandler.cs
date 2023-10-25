using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;
using Newtonsoft.Json;

namespace EventBus
{
    public class EventBusHandler : IEventBus

    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EventBusHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish(IntegrationEvent @event )
        {
            @event.eventContent = JsonConvert.SerializeObject(@event);

            await _publishEndpoint.Publish(@event);


        }
    }
}