using MediatR;
using Ordering.Domain.Sahred;

namespace Ordering.Application.Idempotency
{
    public abstract record  IdempotencyCommand(Guid requestId): IRequest<Result>;
   
}
