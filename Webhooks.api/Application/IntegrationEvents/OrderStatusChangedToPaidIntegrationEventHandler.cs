using EventBus.Abstraction;
using EventBus.Events;
using EventBus.IntegrationEvents;
using IntegrationEventLogEF.DbContexts;
using MassTransit;
using Newtonsoft.Json;
using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>

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


    public async Task Consume(ConsumeContext<OrderStatusChangedToPaidIntegrationEvent> context)
    {

        OrderStatusChangedToPaidIntegrationEvent @event = context.Message;
        var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderPaid);
        _logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process", subscriptions.Count());
        var whook = new WebhookData(WebhookType.OrderPaid, @event);
        await _sender.SendAll(subscriptions, whook);
    }
}


