

using Ordering.Domain.Events;
using Ordering.Domain.Prematives;
using Ordering.Domain.Sahred;

namespace Ordering.Domain.AggregatesModel.OrderAggregate;
public class Order : AggregateRoot
{
    private readonly HashSet<OrderItem> _orderItems;
    public OrderStatus OrderStatus { get; private set; }
    public Address Address { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public int? GetBuyerId => _buyerId;

    private int _orderStatusId;
    private int? _paymentMethodId;
    private DateTime _orderDate;
    private int? _buyerId;
    private string _description;

    private Order( Address address, int? buyerId = null, int? paymentMethodId = null) :this()
    {
        _buyerId = buyerId;
        _paymentMethodId = paymentMethodId;
        _orderStatusId = OrderStatus.Submitted.Id;
        _orderDate = DateTime.UtcNow;
        Address = address;

    }
    protected Order()
    {
        _orderItems = new();
    }
    private  void AddOrderStartedDomainEvent( string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(userName);

        this.AddDomainEvent(orderStartedDomainEvent);

    }


    public static Order CreateOrder( Address address, string userId, string userName, int cardTypeId, string cardNumber, string cardSecurityNumber,
            string cardHolderName, DateTime cardExpiration, int? buyerId = null, int? paymentMethodId = null)
    {
        Order order = new Order( address, buyerId, paymentMethodId);
        order.AddOrderStartedDomainEvent( userName, cardTypeId, cardNumber,
                                    cardSecurityNumber, cardHolderName, cardExpiration);
        return order;
    }
    public void SetPaymentId(int id)
    {
        _paymentMethodId = id;
    }

    public void SetBuyerId(int id)
    {
        _buyerId = id;
    }
    public Result AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
            .SingleOrDefault();

        if (existingOrderForProduct != null)
        {
            //if previous line exist modify it with higher discount  and units..

            if (discount > existingOrderForProduct.GetCurrentDiscount())
            {
                existingOrderForProduct.SetNewDiscount(discount);
            }

            existingOrderForProduct.AddUnits(units);
            return Result.success();
        }
        else
        {
            //add validated new order item

            var orderItem =  OrderItem.Create(productId, productName, unitPrice, discount, pictureUrl, units);
            if (orderItem.IsSuccess)
            {
                _orderItems.Add(orderItem.Value);
                return Result.success();

            }
            return Result.Failure<OrderItem>(orderItem.Error);
        }
    }
}
