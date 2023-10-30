using EventBus.Abstraction;
using EventBus.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Ordering.SignalrHub.Hubs;
using Ordering.SignalrHub.IntegrationEventHandling.IntegrationEvents;

namespace Ordering.SignalrHub.IntegrationEventHandling.EventHandling
{
    public  class OrderStatusChangedToSubmittedIntegrationEventHandler :
        IIntegrationEventHandler<IntegrationEvent>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;
        public OrderStatusChangedToSubmittedIntegrationEventHandler(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<IntegrationEvent> context)
        {
            OrderStatusChangedToSubmittedIntegrationEvent @event = JsonConvert.DeserializeObject<OrderStatusChangedToSubmittedIntegrationEvent>(context.Message.eventContent);
            await _hubContext.Clients.
                           Group(@event.BuyerName)
                           .SendAsync("UpdatedOrderState", new { OrderId = @event.OrderId, Status = @event.OrderStatus });

            //UpdatedOrderState is the method that will invoke in client side when message sent to group

            
        }


    }
}
