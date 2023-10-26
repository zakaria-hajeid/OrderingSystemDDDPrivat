using EventBus.Events;
namespace Webhooks.api.Application.IntegrationEvents;
public sealed record OrderStatusChangedToPaidIntegrationEvent(int OrderId, List<OrderStockItem> orderStockItems) :
    IntegrationEvent;
public record OrderStockItem
{
    public int ProductId { get; }
    public int Units { get; }

    public OrderStockItem(int productId, int units)
    {
        ProductId = productId;
        Units = units;
    }
}