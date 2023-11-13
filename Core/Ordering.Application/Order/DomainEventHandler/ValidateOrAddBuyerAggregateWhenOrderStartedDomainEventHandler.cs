using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Order.IntegrationEvents;
using Ordering.Application.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Events;
using Ordering.Domain.Repository;
using System.Reflection;

namespace Ordering.Application.Order.DomainEventHandler;
internal sealed class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
                    : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly ILogger _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
    private readonly IBuyerService _buyerService;

    public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
        ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService,
        IBuyerService buyerService)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buyerService = buyerService;
    }

    public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {

        var byerCreate = await _buyerService.UpdateOrCreate(domainEvent);
        if (byerCreate.IsFailuer)
        {
            throw new InvalidOperationException(byerCreate.Error.Message);
        }
        //publish domain event and save changes
        await _buyerRepository.UnitOfWork.PublishEventAsyncAsync(cancellationToken);
        var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(domainEvent.order.Id, domainEvent.order.OrderStatus.Name, byerCreate.Value.Name);
        integrationEvent.assymblyName= Assembly.GetExecutingAssembly().FullName!;
        await _orderingIntegrationEventService.SaveEventAsync(integrationEvent);
        // OrderingApiTrace.LogOrderBuyerAndPaymentValidatedOrUpdated(_logger, buyerUpdated.Id, domainEvent.Order.Id);
    }
}
