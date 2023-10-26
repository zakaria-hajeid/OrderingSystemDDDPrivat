using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application;

public interface IWebhooksSender
{
    Task SendAll(IEnumerable<WebhookSubscription> receivers, WebhookData data);
}
