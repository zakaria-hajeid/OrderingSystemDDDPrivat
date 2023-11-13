using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Errors;
using Ordering.Domain.Events;
using Ordering.Domain.Repository;
using Ordering.Domain.Sahred;
using Ordering.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Services.DomainServices;

public class BuyerService : IBuyerService
{
    private readonly IBuyerRepository _buyerRepository;
    public BuyerService(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }
    public async Task<Result<Buyer>> UpdateOrCreate(OrderStartedDomainEvent @event)
    {
        var buyer = await _buyerRepository.FindAsync(@event.userId);
        if (buyer == null)
        {
            if (string.IsNullOrWhiteSpace(@event.userId))
            {
                return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(@event.userId)));
            }
            if (string.IsNullOrWhiteSpace(@event.userName))
            {
                return Result.Failure<Buyer>(DomainErrors.ErrorWithParameters(DomainErrors.NullArgumentsError, nameof(@event.userName)));
            }
            buyer = Buyer.create(@event.userId, @event.userName);

        }
        //VerifyOrAddPaymentMethod
        var tryVerify = buyer.VerifyOrAddPaymentMethod(@event);
        if (tryVerify.IsFailuer)
        {
            return Result.Failure<Buyer>(tryVerify.Error);
        }
        await _buyerRepository.AddOrUpdate(buyer);
        return buyer;
    }

}

