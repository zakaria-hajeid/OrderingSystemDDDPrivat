using MediatR;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Abstraction.Messaging
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
       // public bool isTransaction { get;  }
    }
    public interface ICommandHandler<TCommand, TRespons> : IRequestHandler<TCommand, Result<TRespons>>
                where TCommand : ICommand<TRespons>

    {
        public bool isTransaction { get; }

    }


}