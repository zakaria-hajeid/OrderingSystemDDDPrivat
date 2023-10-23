using Ordering.Domain.Errors;
using Ordering.Domain.Events;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Domain.Sahred;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : AggregateRoot
{
    public string IdentityGuid { get; private set; }

    public string Name { get; private set; }

    private List<PaymentMethod> _paymentMethods;

    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer()
    {

        _paymentMethods = new List<PaymentMethod>();
    }

    private Buyer(string identity, string name) : this()
    {
        IdentityGuid = identity;
        Name = name;
    }

    /* int cardTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration, int orderId*/
    private Result<PaymentMethod> VerifyOrAddPaymentMethod(OrderStartedDomainEvent @event)
    {
        var cardTypeId = @event.cardTypeId != 0 ? @event.cardTypeId : 1;

        var existingPayment = _paymentMethods
            .SingleOrDefault(p => p.IsEqualTo(cardTypeId, @event.cardNumber, @event.cardExpiration));

        if (existingPayment != null)
        {
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, @event.order.Id));

            return existingPayment;
        }

        var payment = PaymentMethod.Create(cardTypeId, $"Payment Method on {DateTime.UtcNow}", @event.cardNumber, @event.cardSecurityNumber, @event.cardHolderName, @event.cardExpiration);
        if (payment.IsSuccess)
        {
            _paymentMethods.Add(payment.Value);
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment.Value, @event.order.Id));
            return payment;

        }
        else
        {
            return payment;
        }


    }

    public async static Task<Result<Buyer>> UpdateOrCreate(OrderStartedDomainEvent @event, IBuyerRepository buyerRepository)
    {
        var buyer = await buyerRepository.FindAsync(@event.userId);
        if (buyer == null)
        {
            if (string.IsNullOrWhiteSpace(@event.userId))
            {
                return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(@event.userId)));
            }
            if (string.IsNullOrWhiteSpace(@event.userName))
            {
                return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(@event.userName)));
            }
            buyer = new(@event.userId, @event.userName);
        }
        //VerifyOrAddPaymentMethod
        var tryVerify = buyer.VerifyOrAddPaymentMethod(@event);
        if (tryVerify.IsFailuer)
        {
            return Result.Failure<Buyer>(tryVerify.Error);
        }
        await buyerRepository.AddOrUpdate(buyer);
        return buyer;

    }
}
