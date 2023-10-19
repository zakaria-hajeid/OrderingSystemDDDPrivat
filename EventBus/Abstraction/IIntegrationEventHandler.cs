using EventBus.Events;
using MassTransit;

namespace EventBus.Abstraction;

public interface IIntegrationEventHandler<in TIntegrationEvent> : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent 
{
}


