using MediatR;
using Ordering.Domain.Prematives;

namespace Ordering.Domain.Prematives;

public abstract class AggregateRoot : Entity {

   
    
   private List<IDomainEvent> _domainEvents = new();
   public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }
    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}


