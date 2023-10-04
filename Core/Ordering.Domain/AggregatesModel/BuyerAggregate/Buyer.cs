using Ordering.Domain.Errors;
using Ordering.Domain.Events;
using Ordering.Domain.Prematives;
using Ordering.Domain.Sahred;
using System.Runtime.InteropServices;

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

    public Result<PaymentMethod> VerifyOrAddPaymentMethod(
        int cardTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration, int orderId)
    {
        var existingPayment = _paymentMethods
            .SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

        if (existingPayment != null)
        {
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

            return existingPayment;
        }

        var payment =  PaymentMethod.Create(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
        if (payment.IsSuccess)
        {
            _paymentMethods.Add(payment.Value);
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment.Value, orderId));
            return payment;

        }
        else
        {
            return payment;
        }


    }

    public static Result<Buyer> Create(string identity, string name)
    {
        if (string.IsNullOrWhiteSpace(identity))
        {
            return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(identity)));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(name)));
        }
        Buyer buyerbuyer = new(identity, name);
        return buyerbuyer;

    }
}
