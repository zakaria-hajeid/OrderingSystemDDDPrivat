using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest :
        IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ApplicationDbContext _applicationDbContext;



        public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger, IOrderingIntegrationEventService orderingIntegrationEventService, ApplicationDbContext applicationDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderingIntegrationEventService = orderingIntegrationEventService;
            _applicationDbContext = applicationDbContext;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = request.GetType().Name;
            try
            {
                switch (request)
                {
                    case ICommand c:
                        if (c.isTransaction)
                        {

                            try
                            {
                                if (_unitOfWork.HasActiveTransaction)
                                {
                                    return await next();
                                }
                                var strategy = _unitOfWork.CreateExecutionStrategy();

                                await strategy.ExecuteAsync(async () =>
                                {
                                    Guid transactionId;

                                    await using var transaction = await _applicationDbContext.BeginTransactionAsync();
                                    using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                                    {
                                        _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                                        response = await next();

                                        _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);
                                        await _applicationDbContext.CommitTransactionAsync(transaction);
                                        transactionId = transaction.TransactionId;
                                    }
                                    // handle publish failier   using backGroundJob 
                                     await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                                });

                                return response;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);
                                throw ex;
                            }
                        }
                        else
                        {
                            response = await next();
                            return response;
                        }
                    default:
                        response = await next();
                        return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);
                throw;
            }

        }
    }
}
