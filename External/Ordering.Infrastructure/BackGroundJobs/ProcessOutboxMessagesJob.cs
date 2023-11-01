using EventBus.Abstraction;
using IntegrationEventLogEF.Entities;
using IntegrationEventLogEF.Enums;
using IntegrationEventLogEF.Services;
using MassTransit;
using MassTransit.Middleware;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using Ordering.Application.Behaviors;
using Ordering.Application.Services;
using Ordering.Persistence;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.BackGroundJobs
{
 [DisallowConcurrentExecution]
    public class ProcessOutboxMessagesJob : IJob
    {
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ILogger<ProcessOutboxMessagesJob> _logger;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;
        private readonly IEventBus _eventBus;
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ApplicationDbContext _orderingContext;


        public ProcessOutboxMessagesJob(IOrderingIntegrationEventService orderingIntegrationEventService, ILogger<ProcessOutboxMessagesJob> logger, ResiliencePipelineProvider<string> resiliencePipelineProvider, IEventBus eventBus, Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory, IIntegrationEventLogService eventLogService, ApplicationDbContext orderingContext)
        {
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _logger = logger;
            _resiliencePipelineProvider = resiliencePipelineProvider;
            _eventBus = eventBus;
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory;
            _eventLogService = _integrationEventLogServiceFactory(_orderingContext.DbConnection()!); 
            _orderingContext = orderingContext;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // AddCheckConstraintOperation manulyy or in dependency 
            /* var pipline = new ResiliencePipelineBuilder()
                 .AddRetry(new RetryStrategyOptions
                 {
                     MaxRetryAttempts = 2,
                     Delay = TimeSpan.Zero,
                     ShouldHandle = new PredicateBuilder()
                     .Handle<ApplicationException>(),
                     OnRetry = r =>
                     {
                         _logger.LogInformation($"retry number {r.AttemptNumber}");
                         return ValueTask.CompletedTask;

                     }
                 }).Build;*/

            var pipline = _resiliencePipelineProvider.GetPipeline("Fault-Event-Publish");
            //try it to show result  await  pipline.ExecuteOutcomeAsync

            
            await pipline.ExecuteAsync(async token =>
            {
                await this.publishFaildEvent();
            });

        }

        public async Task publishFaildEvent()
        {
            var failedEvent = await _orderingIntegrationEventService.GetFailedIntegartionEvents();
            foreach (var @event in failedEvent)
            {
                try
                {
                    await _eventLogService.UpdateEventState(@event.EventId, EventStateEnum.InProgress);
                    await _eventBus.Publish(@event.IntegrationEvent);
                    await _eventLogService.UpdateEventState(@event.EventId, EventStateEnum.Published);
                }

                catch (Exception ex)
                {
                    await _eventLogService.UpdateEventState(@event.EventId, EventStateEnum.PublishedFailed);
                    throw ex;// to retry streatgy

                }
            }



        }
    }
}
