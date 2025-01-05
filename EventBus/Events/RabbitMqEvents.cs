using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events
{
    public abstract record RabbitMqEvents : IntegrationEvent
    {
        public RabbitMqEvents() : base()
        {
        }
        public abstract string QueueName { get; }
        public short Channel { get; set; }
   
    }
}

