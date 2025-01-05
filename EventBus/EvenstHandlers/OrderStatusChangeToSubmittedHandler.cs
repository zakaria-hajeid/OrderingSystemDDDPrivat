using EventBus.Abstraction;
using EventBus.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.EvenstHandlers
{
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
