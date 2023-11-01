using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Idempotency
{
    public abstract record  IdempotencyCommand(Guid requestId):IRequest;
   
}
