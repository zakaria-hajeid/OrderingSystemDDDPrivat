using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Prematives;

namespace Ordering.Domain.Repository;

public interface IBuyerRepository : IRepository<Buyer>

{
    public IUnitOfWork UnitOfWork { get; set; }

    Task<Buyer> Add(Buyer buyer);
    Task<Buyer> FindAsync(string BuyerIdentityGuid);
    Task<Buyer> FindByIdAsync(string id);

}

