using EventBus.Events;

namespace Ordering.Application.Services;

public interface IOrderingIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task SaveEventAsync(IntegrationEvent evt);
}
