using EventBus.Events;
namespace Ordering.SignalrHub.IntegrationEventHandling.IntegrationEvents;
public sealed record OrderStatusChangedToSubmittedIntegrationEvent(int OrderId, string OrderStatus, string BuyerName) :
    IntegrationEvent;




