using EventBus.Events;

namespace EventBus.IntegrationEvents;
public sealed record OrderStatusChangedToPaidIntegrationEvent : RabbitMqEvents
{
    public OrderStatusChangedToPaidIntegrationEvent()
    {
    }

    public int OrderId { get; private set; }
    public int BuyerId { get; private set; }
    public List<OrderStockItem> orderStockItems { get; set; } = new();

    public override string QueueName => "Order";

    public override void setEventType()
    {
        EventType = (int)EventTypeNameEnum.OrderStatusChangedToPaidIntegrationEvent;
    }
}
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