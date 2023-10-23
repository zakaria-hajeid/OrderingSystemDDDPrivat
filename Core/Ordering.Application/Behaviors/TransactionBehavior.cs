using MediatR;
using Ordering.Application.Abstraction.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest :
        IRequest<TResponse>
    {
        public TransactionBehavior()
        {
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
               /* switch (request)
                {
                    case ICommand c:
                      if(c.isTransaction)
                        {
                            //open 
                        }

                        break;
                }*/

                //or switch expression 


                if (request is ICommand)
                {
                    ICommand requests = (ICommand)request;

                    if (requests.isTransaction)
                    {
                        //Open Transaction Bhavior
                    }
                }
                else
                {
                    await next();
                }
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();

            }

        }
    }
}
