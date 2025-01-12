using EventBus.Events;
using EventBus.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Utility
{
    public static class IntegrationEventsLoockup
    {
        public static Dictionary<EventTypeNameEnum, dynamic> IntegrationEventsLoockUp = new Dictionary<EventTypeNameEnum, dynamic>() {
            { EventTypeNameEnum.OrderStatusChangedToPaidIntegrationEvent,new OrderStatusChangedToPaidIntegrationEvent() },
            { EventTypeNameEnum.OrderStatusChangedToSubmittedIntegrationEvent,new OrderStatusChangedToSubmittedIntegrationEvent() },
        };



        public static Dictionary<EventTypeNameEnum, Type> IntegrationEventsLoockUpRabbitMQ = new Dictionary<EventTypeNameEnum, Type>() {
            { EventTypeNameEnum.OrderStatusChangedToPaidIntegrationEvent,typeof( OrderStatusChangedToPaidIntegrationEvent) },
            { EventTypeNameEnum.OrderStatusChangedToSubmittedIntegrationEvent,typeof( OrderStatusChangedToSubmittedIntegrationEvent) },
        };

    }
}
