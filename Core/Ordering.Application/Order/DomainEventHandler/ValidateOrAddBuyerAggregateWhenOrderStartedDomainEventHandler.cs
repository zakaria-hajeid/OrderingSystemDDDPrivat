using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Order.IntegrationEvents;
using Ordering.Application.Services;
using Ordering.Domain.Events;
using Ordering.Domain.Repository;

namespace Ordering.Application.Order.DomainEventHandler;
internal sealed class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
                    : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly ILogger _logger;
    private readonly IBuyerRepository _buyerRepository;
     private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
        ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger,
        IBuyerRepository buyerRepository,
        IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var cardTypeId = domainEvent.cardTypeId != 0 ? domainEvent.cardTypeId : 1;
        var buyer = await _buyerRepository.FindAsync(domainEvent.userId);
        var buyerExisted = buyer is not null;

        if (!buyerExisted)
        {
            //  buyer = new Buyer(domainEvent.UserId, domainEvent.UserName);
        }

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                        $"Payment Method on {DateTime.UtcNow}",
                                        domainEvent.cardNumber,
                                        domainEvent.cardSecurityNumber,
                                        domainEvent.cardHolderName,
                                        domainEvent.cardExpiration,
                                        domainEvent.order.Id);

        var buyerUpdated = buyerExisted ?
            _buyerRepository.Update(buyer) :
            _buyerRepository.Add(buyer);

        await _buyerRepository.UnitOfWork
            .SaveEntitiesAsync(cancellationToken);

        var integrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(domainEvent.order.Id, domainEvent.order.OrderStatus.Name, buyer.Name);
        await _orderingIntegrationEventService.SaveEventAsync(integrationEvent);
       // OrderingApiTrace.LogOrderBuyerAndPaymentValidatedOrUpdated(_logger, buyerUpdated.Id, domainEvent.Order.Id);
    }
}
