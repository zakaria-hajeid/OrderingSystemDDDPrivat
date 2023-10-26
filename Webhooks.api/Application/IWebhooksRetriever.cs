using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application;

public interface IWebhooksRetriever
{
    Task<IEnumerable<WebhookSubscription>> GetSubscriptionsOfType(WebhookType type, int buyerId);
}
