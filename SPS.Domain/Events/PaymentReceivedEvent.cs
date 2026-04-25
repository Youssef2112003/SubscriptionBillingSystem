using SPS.Domain.Common;

namespace SPS.Domain.Events;

public record PaymentReceivedEvent(
    Guid InvoiceId,
    Guid SubscriptionId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}