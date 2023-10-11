using IntegrationEventLogEF.Entities;

namespace IntegrationEventLogEF.Services;

public interface IIntegrationEventLogService
{
    Task<IEnumerable<IntegrationEventOutbox>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
    Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
    Task MarkEventAsPublishedAsync(Guid eventId);
    Task MarkEventAsInProgressAsync(Guid eventId);
    Task MarkEventAsFailedAsync(Guid eventId);
}
