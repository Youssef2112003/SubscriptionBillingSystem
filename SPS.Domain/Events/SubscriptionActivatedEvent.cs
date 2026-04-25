using SPS.Domain.Common;

namespace SPS.Domain.Events;

public record SubscriptionActivatedEvent(
    Guid SubscriptionId,
    Guid CustomerId,
    Guid FirstInvoiceId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}