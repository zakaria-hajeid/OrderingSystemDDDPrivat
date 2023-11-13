using Ordering.Domain.Events;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public  interface IBuyerService
    {
        Task<Result<Buyer>> UpdateOrCreate(OrderStartedDomainEvent @event);
    }
}
