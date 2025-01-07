using EventBus.Abstraction.RabbitMq;
using EventBus.IntegrationEvents;

namespace Ordering.SignalrHub.RabbitMqHandlers.EvenstHandlers
{
    // todo move to handler microservices 
    public class OrderStatusChangeToSubmittedHandler : IRabbitMqEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        public OrderStatusChangeToSubmittedHandler()
        {
            
        }
        public async ValueTask<bool> HandleAsync(OrderStatusChangedToSubmittedIntegrationEvent parameters)
        {
            return true ;                
            
        }
    }
}
