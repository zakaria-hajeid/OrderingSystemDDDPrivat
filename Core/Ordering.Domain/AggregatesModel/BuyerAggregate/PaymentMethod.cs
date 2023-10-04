using Ordering.Domain.Errors;
using Ordering.Domain.Exceptions;
using Ordering.Domain.Prematives;
using Ordering.Domain.Sahred;
using System.Security.Principal;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate;

public class PaymentMethod : Entity
{
    private string _alias;
    private string _cardNumber;
    private string _securityNumber;
    private string _cardHolderName;
    private DateTime _expiration;

    private int _cardTypeId;
    public CardType CardType { get; private set; }

    protected PaymentMethod() { }

    private PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        _cardNumber = cardNumber;
        _securityNumber = securityNumber;
        _cardHolderName =  cardHolderName ;
        _alias = alias;
        _expiration = expiration;
        _cardTypeId = cardTypeId;
    }

    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return _cardTypeId == cardTypeId
            && _cardNumber == cardNumber
            && _expiration == expiration;
    }
    public static Result<PaymentMethod> Create(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return Result.Failure<PaymentMethod>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(cardNumber)));
        }
        if (string.IsNullOrWhiteSpace(securityNumber))
        {
            return Result.Failure<PaymentMethod>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(securityNumber)));
        }
        if (string.IsNullOrWhiteSpace(cardHolderName))
        {
            return Result.Failure<PaymentMethod>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(cardHolderName)));
        }
        if (expiration < DateTime.UtcNow)
        {
            return Result.Failure<PaymentMethod>(DomainErrors.PaymentMethodError.CardExpire);
        }
        PaymentMethod paymentMethod = new(cardTypeId, alias,cardNumber,securityNumber,cardHolderName,expiration);
        return paymentMethod;

    }
}
