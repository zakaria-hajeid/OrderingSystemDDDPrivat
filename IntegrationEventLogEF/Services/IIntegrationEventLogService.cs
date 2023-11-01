using IntegrationEventLogEF.Entities;

namespace IntegrationEventLogEF.Services;

public interface IIntegrationEventLogService
{
    Task<IEnumerable<IntegrationEventOutbox>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
    Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
    Task UpdateEventState(Guid eventId, EventStateEnum eventStateEnum );
    Task<IEnumerable<IntegrationEventOutbox>> RetrieveFailedPublishEvent();

}
