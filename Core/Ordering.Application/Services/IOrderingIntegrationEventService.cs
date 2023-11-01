using EventBus.Events;
using IntegrationEventLogEF.Entities;

namespace Ordering.Application.Services;

public interface IOrderingIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task SaveEventAsync(IntegrationEvent evt);
    Task<IEnumerable<IntegrationEventOutbox>> GetFailedIntegartionEvents();
}
