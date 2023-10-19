using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;

namespace EventBus
{
    public class EventBusHandler : IEventBus

    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EventBusHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish(IntegrationEvent @event)
        {
            await _publishEndpoint.Publish(@event);


        }
    }
}