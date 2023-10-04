using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Prematives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Events
{
    public  sealed record BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, int orderId) :IDomainEvent;
    
}
