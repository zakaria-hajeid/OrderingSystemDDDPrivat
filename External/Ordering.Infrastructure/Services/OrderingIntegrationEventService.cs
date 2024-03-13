using EventBus;
using EventBus.Abstraction;
using EventBus.Events;
using EventBus.Utility;
using IntegrationEventLogEF.Entities;
using IntegrationEventLogEF.Enums;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Persistence;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;

namespace Ordering.Infrastructure.Services;

public class OrderingIntegrationEventService : IOrderingIntegrationEventService
{

    //<summary>
    //هاي عبارة عن لايبراري بتعالج الاينتيجراشين  الايفنتس عشان الكل يستخدمها
    //بستخدمعها مشان اتعامل مع الايفنت اللي عندي لانو الايفنت شيرنج
    //بين كل المايكروسيرفس
    //والها كونتيكست خاص فيها بقدر اعملو سكيما عنفس الداتا بيس الي انا فاتحه ا حسب المايكروسيرفس 
    //</summary>

    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
    private readonly IEventBus _eventBus;
    private readonly ApplicationDbContext _orderingContext;

    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<OrderingIntegrationEventService> _logger;

    public OrderingIntegrationEventService(IEventBus eventBus,
        ApplicationDbContext orderingContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
        ILogger<OrderingIntegrationEventService> logger,
        IUnitOfWork unitOfWork)
    {
        _orderingContext = orderingContext;
        _integrationEventLogServiceFactory = integrationEventLogServiceFactory;
        _eventBus = eventBus;
        _eventLogService = _integrationEventLogServiceFactory(_orderingContext.DbConnection()!);
        _logger = logger;
    }

    public async Task<IEnumerable<IntegrationEventOutbox>> GetFailedIntegartionEvents()
    {
        var pendingLogEvents = await _eventLogService.RetrieveFailedPublishEvent();

        return pendingLogEvents;
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    {
        var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var logEvt in pendingLogEvents)
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvt.EventId, logEvt.IntegrationEvent);

            try
            {
                await _eventLogService.UpdateEventState(logEvt.EventId, EventStateEnum.InProgress);

                dynamic? integrationEvent = retriveIntegrationEventByType(logEvt.EventType);
                if (integrationEvent is null)
                {
                    throw new InvalidOperationException("Integration event is not found");
                }
                integrationEvent = logEvt.IntegrationEvent;
                await _eventBus.Publish(integrationEvent);
                await _eventLogService.UpdateEventState(logEvt.EventId, EventStateEnum.Published);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);

               await _eventLogService.UpdateEventState(logEvt.EventId, EventStateEnum.PublishedFailed);
            }
        }
    }
    public async Task SaveEventAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

        await _eventLogService.SaveEventAsync(evt, _orderingContext.GetCurrentTransaction());
    }
    private dynamic? retriveIntegrationEventByType(EventTypeNameEnum eventTypeNameEnum)
    {
        dynamic integrationEvent;
        integrationEvent = IntegrationEventsLoockup.IntegrationEventsLoockUp.Where(x => x.Key == eventTypeNameEnum).First();
        return integrationEvent;
    }
}
