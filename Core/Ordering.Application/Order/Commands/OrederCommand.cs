using Ordering.Application.Abstraction.Messaging;
using Ordering.Application.Dtos.CreateOrderDtos;
using Ordering.Application.Idempotency;

namespace Ordering.Application.Order.Commands
{
    /* public sealed record OrederCommand(Guid requestId, string UserId, string UserName, string City, string Street, string State, string ZipCode,
         string Country, string CardNumber, string CardHolderName, DateTime CardExpiration, string CardSecurityNumber,
         int CardTypeId, IEnumerable<OrderItemDto> orderItemDtos) : IdempotencyCommand(requestId), ICommand
     {
         public bool isTransaction => true;

     }
    */

    public sealed record OrederCommand : IdempotencyCommand, ICommand
    {
        public OrederCommand() :base(Guid.NewGuid())
        {
            
        }
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }
        public IEnumerable<OrderItemDto> orderItemDtos { get; set; }

        public bool isTransaction => true;
    }

}
