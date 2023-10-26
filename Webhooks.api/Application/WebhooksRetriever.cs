using Microsoft.EntityFrameworkCore;
using Webhooks.api.Persistence;
using Webhooks.api.Persistence.Model;

namespace Webhooks.api.Application;

public class WebhooksRetriever : IWebhooksRetriever
{
    private readonly WebhooksContext _db;
    public WebhooksRetriever(WebhooksContext db)
    {
        _db = db;
    }
    public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsOfType(WebhookType type)
    {
        var data = await _db.Subscriptions.Where(s => s.Type == type).ToListAsync();
        return data;
    }
}
