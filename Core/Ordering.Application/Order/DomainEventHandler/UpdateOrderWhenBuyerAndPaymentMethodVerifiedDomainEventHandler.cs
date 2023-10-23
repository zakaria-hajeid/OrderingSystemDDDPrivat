using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using Ordering.Domain.Repository;

namespace Ordering.Application.Order.DomainEventHandler;

public sealed class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
        IOrderRepository orderRepository,
        ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    // Domain Logic comment:
    // When the Buyer and Buyer's payment method have been created or verified that they existed, 
    // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(domainEvent.orderId);
        if (orderToUpdate != null)
        {
            orderToUpdate.SetBuyerId(domainEvent.buyer.Id);
            orderToUpdate.SetPaymentId(domainEvent.payment.Id);
        }
        else
        {
            throw new ArgumentNullException($"The Order Id {domainEvent.orderId} Is not exist");
        }
        // OrderingApiTrace.LogOrderPaymentMethodUpdated(_logger, domainEvent.OrderId, nameof(domainEvent.Payment), domainEvent.Payment.Id);
    }
}
