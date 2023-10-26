using Microsoft.EntityFrameworkCore;
using Webhooks.api.Persistence.Model;
using static System.Net.WebRequestMethods;

namespace Webhooks.api.Persistence;

public class WebhooksContext : DbContext
{

    public WebhooksContext(DbContextOptions<WebhooksContext> options) : base(options)
    {
    }
    public DbSet<WebhookSubscription> Subscriptions { get; set; }
}


public class WebhooksContextSeedData
{
    public async Task SeedAsync(WebhooksContext webhooksContext)
    {
        using (webhooksContext)
        {
            try
            {
                // TODO :BUILD END POINT TO SET THE WEBHOOK AND ,AKE SURE FROM SAME ORGIN AMD gRANT DEST  uRL 
                WebhookSubscription webhookSubscription = new WebhookSubscription()
                {
                    Date = DateTime.Now,
                    Token = "Password=123",
                    DestUrl = "https://localhost:7264/OrderStock/SetStockItem",
                    UserId = "381",
                    Type = WebhookType.OrderPaid

                };
                bool anySub = await webhooksContext.Subscriptions.Where(x => x.DestUrl == webhookSubscription.DestUrl
                 && x.Type == webhookSubscription.Type ).AnyAsync();

                if (!anySub)
                {
                    webhooksContext.Subscriptions.Add(webhookSubscription);
                    await webhooksContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}


