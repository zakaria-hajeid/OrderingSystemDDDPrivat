using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}
