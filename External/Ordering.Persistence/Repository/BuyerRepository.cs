using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;

namespace Ordering.Persistence.Repository;

public class BuyerRepository
    : Repository<Buyer>, IBuyerRepository
{
    private readonly ApplicationDbContext _context;

    public IUnitOfWork UnitOfWork { get => _context; set => throw new NotImplementedException(); }

    public BuyerRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Buyer> Add(Buyer buyer)
    {
        if (buyer.IsTransient())
        {
            var buyerToAdd = await _context.Buyers
                            .AddAsync(buyer);
            return buyerToAdd.Entity;

        }
        return buyer;
    }
    //Todo specification pattern 
    public async Task<Buyer> FindAsync(string identity)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.IdentityGuid == identity)
            .SingleOrDefaultAsync();

        return buyer;
    }

    public async Task<Buyer> FindByIdAsync(string id)
    {
        var buyer = await _context.Buyers
            .Include(b => b.PaymentMethods)
            .Where(b => b.Id == int.Parse(id))
            .SingleOrDefaultAsync();

        return buyer;
    }
}
