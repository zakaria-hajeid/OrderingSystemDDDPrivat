using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;
using Ordering.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task Add(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<Order> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(Order entityToDelete)
        {
            throw new NotImplementedException();
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Order entityToUpdate)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Order> entities)
        {
            throw new NotImplementedException();
        }
    }
}
