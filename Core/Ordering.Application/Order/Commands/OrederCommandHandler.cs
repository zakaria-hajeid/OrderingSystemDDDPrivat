using MediatR;
using Ordering.Application.Abstraction.Messaging;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Order.Commands
{
    internal sealed class OrederCommandHandler : ICommandHandler<OrederCommand>
    {
        public OrederCommandHandler()
        {

        }
        public Task<Result> Handle(OrederCommand request, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
