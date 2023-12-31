﻿using MediatR;
using Ordering.Domain.Sahred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Abstraction.Messaging
{
    public interface ICommand : IRequest<Result>
    {
        public bool isTransaction { get;}
    }
    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
        public bool isTransaction { get; }

    }
}
