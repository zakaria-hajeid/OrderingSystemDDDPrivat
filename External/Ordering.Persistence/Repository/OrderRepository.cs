using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
namespace Ordering.Persistence.Repository
{
    /// <summary>
    ///     MICROSOFT PERSPECTIVE abou repo  :
    //    EACH REPO HAVE THAIR CONTEXT SQL OREFCORE OR ANY THING AND
    //        EACH CONTEXT IS A UNIT OF WORK WHEN CALL REPO YOU USE
    //        THE UNIT OF WORD PROPERTY(RETERN THE CONTEXT IN THE REPO AND 
    //            ALSO THIS CONTEXT ALREADY UNIT OF WORK) AND SAVE CHANGES
    //MEANS THE REPO WILL DECIDE THE CONTEXT TYPE AND THE UNIT OF WORK THAT WILL WORK WITH IT
    /// </summary>
    public sealed class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public IUnitOfWork UnitOfWork { get => _context; set => throw new NotImplementedException(); }

        public OrderRepository(ApplicationDbContext context):base(context) 
        {
            _context= context;
        }
        

    }
}
