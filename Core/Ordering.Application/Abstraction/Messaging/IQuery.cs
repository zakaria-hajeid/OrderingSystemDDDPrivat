using MediatR;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Abstraction.Messaging
{
    public interface IQuery<TResponse> :IRequest<Result<TResponse>>
    {
    }
}
