using Ordering.Application.Abstraction.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Order.Commands
{
    public sealed record OrederCommand(string orderId) : ICommand;
}
