using EventBus.Events;
namespace EventBus.IntegrationEvents;
public sealed record OrderStatusChangedToSubmittedIntegrationEvent : RabbitMqEvents

{
    public OrderStatusChangedToSubmittedIntegrationEvent()
    {

    }
    public OrderStatusChangedToSubmittedIntegrationEvent(int orderId, string orderStatus, string buyerName)
    {
        OrderId = orderId;
        OrderStatus = orderStatus;
        BuyerName = buyerName;
    }
    public int OrderId { get; init; }
    public string OrderStatus { get; init; }
    public string BuyerName { get; init; }

    public override string QueueName => "Order";

    public override void setEventType()
    {
        EventType = (int)EventTypeNameEnum.OrderStatusChangedToSubmittedIntegrationEvent;
    }
}


