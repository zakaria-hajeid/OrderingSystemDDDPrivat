using EventBus.Abstraction;
using EventBus.Events;
using IntegrationEventLogEF.DbContexts;
using MassTransit;
using Newtonsoft.Json;
using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<IntegrationEvent>

{
    private readonly IWebhooksRetriever _retriever;
    private readonly IWebhooksSender _sender;
    private readonly ILogger _logger;
    public OrderStatusChangedToPaidIntegrationEventHandler(IWebhooksRetriever retriever, IWebhooksSender sender, ILogger<OrderStatusChangedToPaidIntegrationEventHandler> logger)
    {
        _retriever = retriever;
        _sender = sender;
        _logger = logger;
    }
   
   
    public async Task Consume(ConsumeContext<IntegrationEvent> context)
    {
        OrderStatusChangedToPaidIntegrationEvent @event = JsonConvert.DeserializeObject<OrderStatusChangedToPaidIntegrationEvent>(context.Message.eventContent);
        var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderPaid, @event.BuyerId);
        _logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process", subscriptions.Count());
        var whook = new WebhookData(WebhookType.OrderPaid, @event);
        await _sender.SendAll(subscriptions, whook);
    }
}
