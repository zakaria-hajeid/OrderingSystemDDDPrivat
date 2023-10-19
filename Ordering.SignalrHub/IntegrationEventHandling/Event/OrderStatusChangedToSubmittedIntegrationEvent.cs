using EventBus.Events;

namespace Ordering.SignalrHub.IntegrationEventHandling.Event
{
    public  record OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent

    {
        public OrderStatusChangedToSubmittedIntegrationEvent(int OrderId, string OrderStatus, string BuyerName)
        {

        }  
    }

    

}
