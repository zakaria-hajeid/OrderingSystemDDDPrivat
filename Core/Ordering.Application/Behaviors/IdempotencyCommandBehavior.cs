using MassTransit.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Application.Idempotency;
using Ordering.Application.Services;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using Ordering.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviors
{
    public class IdempotencyCommandBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest :
        IdempotencyCommand
    {
        public IdempotencyCommandBehavior( )
        {

        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

        }
    }
}
