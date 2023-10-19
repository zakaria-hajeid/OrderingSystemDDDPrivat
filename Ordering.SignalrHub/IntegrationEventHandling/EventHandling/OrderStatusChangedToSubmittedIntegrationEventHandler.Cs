using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;
using Ordering.SignalrHub.IntegrationEventHandling.Event;

namespace Ordering.SignalrHub.IntegrationEventHandling.EventHandling
{
    public  class OrderStatusChangedToSubmittedIntegrationEventHandler : 
        IIntegrationEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        
        public async Task Consume(ConsumeContext<OrderStatusChangedToSubmittedIntegrationEvent> context)
        {
            throw new NotImplementedException();
        }

        
    }
}
