namespace Ordering.Domain.Prematives;

public interface IRepository<T> where T : AggregateRoot
{
    //in ddd the genrec repo should inhret from aggregate just becuse the aggregate is responsapility for command or query 
    //to chlide
    Task<T?> GetByIdAsync(Guid id);
    Task Add(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entityToUpdate);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entityToDelete);
    void Delete(object id);
    ValueTask AddOrUpdate(T entity);

}
