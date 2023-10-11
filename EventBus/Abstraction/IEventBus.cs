using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstraction
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

    }
}
