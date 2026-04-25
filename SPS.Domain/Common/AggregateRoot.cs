namespace SPS.Domain.Common;

public abstract class AggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot
{
}