using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Idempotency
{
    public interface IdempotencyService
    {
        Task<bool> RequestExist(Guid requestId);
        Task CreateRequest(Guid requestId,string actionName);

    }
}
