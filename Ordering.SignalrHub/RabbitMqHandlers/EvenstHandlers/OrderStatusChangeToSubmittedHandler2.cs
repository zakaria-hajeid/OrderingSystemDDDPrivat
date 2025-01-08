using EventBus.Abstraction.RabbitMq;
using EventBus.IntegrationEvents;

namespace Ordering.SignalrHub.RabbitMqHandlers.EvenstHandlers
{
    // todo move to handler microservices 
    public class OrderStatusChangeToSubmittedHandler2 : IRabbitMqEventHandler<OrderStatusChangedToSubmittedIntegrationEvent>
    {
        public OrderStatusChangeToSubmittedHandler2()
        {
            
        }
        public async ValueTask<bool> HandleAsync(OrderStatusChangedToSubmittedIntegrationEvent parameters)
        {
            return false ;                
            
        }
    }
}
