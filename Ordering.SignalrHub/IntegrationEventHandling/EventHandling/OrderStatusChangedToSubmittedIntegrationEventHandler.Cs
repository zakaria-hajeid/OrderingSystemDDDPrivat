using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;
using Newtonsoft.Json;
using Ordering.SignalrHub.IntegrationEventHandling.IntegrationEvents;

namespace Ordering.SignalrHub.IntegrationEventHandling.EventHandling
{
    public  class OrderStatusChangedToSubmittedIntegrationEventHandler :
        IIntegrationEventHandler<IntegrationEvent>
    {
        
        public async Task Consume(ConsumeContext<IntegrationEvent> context)
        {
            OrderStatusChangedToSubmittedIntegrationEvent c = JsonConvert.DeserializeObject<OrderStatusChangedToSubmittedIntegrationEvent>(context.Message.eventContent);

            throw new Exception(context.Message.assymblyName);
         }

        
    }
}
