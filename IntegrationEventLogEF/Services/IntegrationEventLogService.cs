using IntegrationEventLogEF.Repository;

namespace IntegrationEventLogEF.Services;

public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
{
    private volatile bool _disposedValue;
    private readonly IIntegrationOutboxRepository _integrationOutboxRepository;
    
    public IntegrationEventLogService(Func<IIntegrationOutboxRepository> integrationOutboxRepositoryFactory)
    {
        _integrationOutboxRepository = integrationOutboxRepositoryFactory.Invoke();

    }
    public async Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        var eventLogEntry = IntegrationEventOutbox.AddingNewEvent(@event, transaction.TransactionId);
        await _integrationOutboxRepository.ADDIntegrationEvent(eventLogEntry);
        await _integrationOutboxRepository.SaveChangesAsync(transaction);
    }

    private async Task UpdateEventStatus(Guid eventId, EventStateEnum eventStateEnum)
    {
        IntegrationEventOutbox integrationEventOutbox = await RetrieveEventById(eventId);
        integrationEventOutbox.UpdateEventStatus(eventStateEnum);
        await _integrationOutboxRepository.UpdateIntegrationEvent(integrationEventOutbox);

    }
    public async Task<IntegrationEventOutbox> RetrieveEventById(Guid eventId)
    {
        IntegrationEventOutbox eventLogEntry = await _integrationOutboxRepository.RetrieveEventById(eventId);
        return eventLogEntry;
    }

    public async Task<IEnumerable<IntegrationEventOutbox>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        return await _integrationOutboxRepository.RetrieveEventLogsPendingToPublishAsync(transactionId);
    }

    public async Task UpdateEventState(Guid eventId, EventStateEnum eventStateEnum)
    {
        await UpdateEventStatus(eventId, eventStateEnum);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _integrationOutboxRepository?.DisposeContext();
            }

            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<IEnumerable<IntegrationEventOutbox>> RetrieveFailedPublishEvent()
    {
        return await _integrationOutboxRepository.RetrieveFailedPublishEvent();
    }
}
