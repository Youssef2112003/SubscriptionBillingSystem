using SPS.Domain.Common;

namespace SPS.Domain.Events;

public record InvoiceGeneratedEvent(
    Guid InvoiceId,
    Guid SubscriptionId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}