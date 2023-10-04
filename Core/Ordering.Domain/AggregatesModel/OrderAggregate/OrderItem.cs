using Ordering.Domain.Errors;
using Ordering.Domain.Exceptions;
using Ordering.Domain.Prematives;
using Ordering.Domain.Sahred;

namespace Ordering.Domain.AggregatesModel.OrderAggregate;
public class OrderItem
    : Entity
{
    private string _productName;
  //  private string _pictureUrl;
    private decimal _unitPrice;
    private decimal _discount;
    private int _units;

    public int ProductId { get; private set; }

    public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, int units = 1)
    {

        ProductId = productId;

        _productName = productName;
        _unitPrice = unitPrice;
        _discount = discount;
        _units = units;
        //_pictureUrl = PictureUrl;
    }
    public decimal GetCurrentDiscount()
    {
        return _discount;
    }
    public void SetNewDiscount(decimal discount)
    {
        if (discount < 0)
        {
            throw new OrderingDomainException("Discount is not valid");
        }

        _discount = discount;
    }
    public void AddUnits(int units)
    {
        if (units < 0)
        {
            throw new OrderingDomainException("Invalid units");
        }

        _units += units;
    }

    public static Result<OrderItem> Create(int productId, string productName, decimal unitPrice, decimal discount,
        string PictureUrl, int units = 1)
    {
        if (units <= 0)
        {
            return Result.Failure<OrderItem>(DomainErrors.orderItem.OrderItemDiscountError);
        }

        if ((unitPrice * units) < discount)
        {
            return Result.Failure<OrderItem>(DomainErrors.orderItem.OrderItemInvalidNumberOfUnitsError);
        }

        OrderItem orderItem = new OrderItem(productId, productName, unitPrice, discount, units);
        return orderItem;
    }

}
