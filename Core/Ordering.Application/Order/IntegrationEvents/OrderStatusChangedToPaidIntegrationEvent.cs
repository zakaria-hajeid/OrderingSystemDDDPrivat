using EventBus.Events;
namespace Ordering.Application.Order.IntegrationEvents;
public sealed record OrderStatusChangedToPaidIntegrationEvent(int OrderId,int BuyerId, List<OrderStockItem> orderStockItems) :
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