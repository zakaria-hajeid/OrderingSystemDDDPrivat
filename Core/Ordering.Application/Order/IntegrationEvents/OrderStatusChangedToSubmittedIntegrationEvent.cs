
using EventBus.Events;

namespace Ordering.Application.Order.IntegrationEvents;

public sealed record OrderStatusChangedToSubmittedIntegrationEvent(int OrderId, string OrderStatus, string BuyerName) :
    IntegrationEvent;


