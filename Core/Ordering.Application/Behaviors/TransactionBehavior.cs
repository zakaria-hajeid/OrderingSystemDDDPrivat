using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
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


        public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger, IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderingIntegrationEventService = orderingIntegrationEventService;
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

                                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
                                    using (_logger.BeginScope(new List<KeyValuePair<string, object>> { new("TransactionContext", transaction.TransactionId) }))
                                    {
                                        _logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                                        response = await next();

                                        _logger.LogInformation("Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                                        await _unitOfWork.CommitTransactionAsync(transaction);

                                        transactionId = transaction.TransactionId;
                                    }

                                    await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);
                                });

                                return response;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error Handling transaction for {CommandName} ({@Command})", typeName, request);

                                throw;
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
