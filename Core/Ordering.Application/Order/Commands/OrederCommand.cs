using Ordering.Application.Abstraction.Messaging;
using Ordering.Application.Dtos.CreateOrderDtos;
using Ordering.Application.Idempotency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Order.Commands
{
    public sealed record OrederCommand(Guid requestId, string UserId, string UserName, string City, string Street, string State, string ZipCode,
        string Country, string CardNumber, string CardHolderName, DateTime CardExpiration, string CardSecurityNumber,
        int CardTypeId, IEnumerable<OrderItemDto> orderItemDtos) : IdempotencyCommand(requestId), ICommand
    {
        public bool isTransaction => true;

    }

}
