using EventBus.Abstraction;
using EventBus.Events;
using EventBus.IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Ordering.SignalrHub.Hubs;

namespace Ordering.SignalrHub.IntegrationEventHandling.EventHandling
{
    public  class OrderStatusChangedToSubmittedIntegrationEventHandler :
        IIntegrationEventHandler<EventBus.IntegrationEvents.OrderStatusChangedToSubmittedIntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        public OrderStatusChangedToSubmittedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<EventBus.IntegrationEvents.OrderStatusChangedToSubmittedIntegrationEvent> context)
        {
            OrderStatusChangedToSubmittedIntegrationEvent @event = context.Message;
            await _hubContext.Clients.
                           Group(@event.BuyerName)
                           .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });

            //UpdatedOrderState is the method that will invoke in client side when message sent to group

            
        }


    }
}
