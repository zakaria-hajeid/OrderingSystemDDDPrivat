using IntegrationEventLogEF.Repository;
using IntegrationEventLogEF.Services;
using Ordering.Application.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Infrastructure.BackGroundJobs;
using Ordering.Infrastructure.Services;
using Ordering.Persistence;
using Ordering.Persistence.Repository;
using OrderingSystemDDD.Extinsions;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Quartz;
using System.Data.Common;

namespace OrderingSystemDDD.Configration
{
    public class InftastructiorServiceInstaller : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContextsExtinstions(configuration);
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IBuyerService, BuyerService>();
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                                sp => (DbConnection c) => new IntegrationEventLogService(
                                                               () => new IIntegrationOutboxRepository(c)));
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
            services.AddQuartz(configure =>
            {

                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

                configure
                    .AddJob<ProcessOutboxMessagesJob>(jobKey)
                    .AddTrigger(
                        trigger => trigger.ForJob(jobKey).WithSimpleSchedule(

                            schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));

                //configure.UseMicrosoftDependencyInjectionJobFactory(); is default 
            });

    services.AddResiliencePipeline("Fault-Event-Publish",
                pip =>
                {
                    pip.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 2,
                        Delay = TimeSpan.Zero,
                        ShouldHandle = new PredicateBuilder()
                    .Handle<ApplicationException>(),

                        OnRetry = r =>
                          {
                              Console.WriteLine(r.AttemptNumber);
                              return ValueTask.CompletedTask;

                          },

                    }

                    );

                    pip.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder()
                     .Handle<ApplicationException>(),
                        BreakDuration = TimeSpan.FromSeconds(30),
                        MinimumThroughput = 3,
                        OnOpened = r =>
                          {
                              Console.WriteLine(r.Outcome.Result);
                              return ValueTask.CompletedTask;

                          },
                    });
                    /*pip.AddFallback<object> (new FallbackStrategyOptions<object>
                    {
                        ShouldHandle = new PredicateBuilder()
                    .Handle<ApplicationException>(),
                        FallbackAction = r => Outcome.FromResultAsValueTask(new object())
                    }); */

                    pip.Build();
                }
                );
}


    }
    }
