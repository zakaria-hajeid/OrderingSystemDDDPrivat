using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Prematives;
using System.Collections.Generic;

namespace Ordering.Persistence
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        private readonly DbContext _context;
        private DbSet<T> Dbset
        {
            get { return _context.Set<T>(); }
        }
        public Repository(DbContext context)
        {
            _context = context;
        }
        public async Task Add(T entity)
        {
            await Dbset.AddAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await Dbset.AddRangeAsync(entities);
        }
        public void Update(T entity)
        {
            Dbset.Update(entity);
        }
        public void UpdateRange(IEnumerable<T> entities)
        {
            Dbset.UpdateRange(entities);
        }
        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                Dbset.Attach(entity);
            }
            Dbset.Remove(entity);
        }
        public void Delete(object id)
        {
            var entity = Dbset.Find(id);
            Dbset.Remove(entity);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await Dbset.FindAsync(id);
        }

        public async ValueTask AddOrUpdate(T entity)
        {
            var entry = _context.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                case EntityState.Added:
                    await Dbset.AddAsync(entity);
                    break;
                case EntityState.Modified:
                    Dbset.Update(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}