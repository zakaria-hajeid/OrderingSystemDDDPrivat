using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Errors
{
    public static class DomainErrors
    {
        public static class order
        {
            public static readonly Error createdOrderError = new Error("Order.ErrorWhenCreateOrder", "Error Occured while creating order ");
            public static readonly Error OrderStatusError = new Error("Order.ErrorWhenCreateOrder", "Error Occured while creating order ");
        }
        public static class orderItem
        {

            public static readonly Error OrderItemDiscountError = new Error("OrderItem.discount", "he total of order item is lower than applied discount");
            public static readonly Error OrderItemInvalidNumberOfUnitsError = new Error("OrderItem.InvalidNumberOfUnits", "Invalid number of units ");


        }
        public static class BuyerError
        {



        }
        public static class PaymentMethodError
        {
            public static readonly Error CardExpire = new Error("PaymentMethodError.CardExpire", "The Card is Expire");

        }
        public static readonly Error NullArgumentsError = new Error("NullArgumentsError", "Null Arguments Error");

        public static Error ErrorWithParameters(Error error, string specific)
        {
            StringBuilder specificString = new StringBuilder();
            specificString.Append(error.Message);
            specificString.Append(specific);


            return new Error(error.Code, specificString.ToString());
        }
    }
}
