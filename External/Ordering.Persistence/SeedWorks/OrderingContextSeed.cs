using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;

namespace Ordering.Persistence.SeedWorks;


public class OrderingContextSeed
{
    public async Task SeedAsync(ApplicationDbContext context, ILogger<OrderingContextSeed> logger)
    {
        //var policy = CreatePolicy(logger, nameof(OrderingContextSeed));

        /*await policy.ExecuteAsync(async () =>
        {

            using (context)
            {
                context.Database.Migrate();
                
                if (!context.CardTypes.Any())
                {
                    context.CardTypes.AddRange(GetPredefinedCardTypes());

                    await context.SaveChangesAsync();
                }

                if (!context.OrderStatus.Any())
                {
                    context.OrderStatus.AddRange(etPredefinedOrderStatus());
                }
                await context.SaveChangesAsync();
            }
        });*/
        using (context)//Use th polly 
        {
            context.Database.Migrate();
            if (!context.CardTypes.Any())
            {
                context.CardTypes.AddRange(GetPredefinedCardTypes());

                await context.SaveChangesAsync();
            }

            if (!context.OrderStatus.Any())
            {
                context.OrderStatus.AddRange(GetPredefinedOrderStatus());
            }
            await context.SaveChangesAsync();
        }
    }
    private IEnumerable<CardType> GetPredefinedCardTypes()
    {
        return Enumeration.GetAll<CardType>();
    }

    private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
    {
        return new List<OrderStatus>()
        {
            OrderStatus.Submitted,
            OrderStatus.AwaitingValidation,
            OrderStatus.StockConfirmed,
            OrderStatus.Paid,
            OrderStatus.Shipped,
            OrderStatus.Cancelled
        };
    }

    /* private AsyncRetryPolicy CreatePolicy(ILogger<OrderingContextSeed> logger, string prefix, int retries = 3)
     {
    // using polly 
         return Policy.Handle<SqlException>().
             WaitAndRetryAsync(
                 retryCount: retries,
                 sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                 onRetry: (exception, timeSpan, retry, ctx) =>
                 {
                     logger.LogWarning(exception, "[{prefix}] Error seeding database (attempt {retry} of {retries})", prefix, retry, retries);
                 }
             );
     }*/
}
