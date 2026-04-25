namespace SPS.Shared.Domain
{
    public abstract class AggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot/*, IInternalEventHandler*/
    {

        //private readonly List<IDomainEvent> _domainEvents = new();

        //public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        //protected void RaiseDomainEvent(IDomainEvent domainEvent)
        //{
        //    _domainEvents.Add(domainEvent);
        //}

        //public void ClearDomainEvents()
        //{
        //    _domainEvents.Clear();
        //}
        //public void ReplayEvents(IEnumerable<object> events)
        //{
        //    foreach (var @event in events)
        //    {
        //        Apply(@event);
        //    }
        //}

        //public IEnumerable<IDomainEvent> GetDomainEvents()
        //{
        //    return _domainEvents;
        //}


        //private readonly List<object> _changes = new();

        //protected abstract void When(object @event);

        //protected void Apply(object @event)
        //{
        //    When(@event);
        //    EnsureValidState();
        //    _changes.Add(@event);
        //}

        //protected void ApplyToEntity(IInternalEventHandler entity, object @event)
        //{
        //    entity?.Handle(@event);
        //}

        //public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

        //public void ClearChanges() => _changes.Clear();

        //protected abstract void EnsureValidState();

        //void IInternalEventHandler.Handle(object @event) => When(@event);
    }
}
