using SPS.Domain.Common;
using SPS.Domain.Exceptions;

namespace SPS.Domain.Entities;

public class Invoice : BaseEntity<Guid>
{
    public Guid SubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime? PaidDate { get; private set; }

    private Invoice() { }

    internal Invoice(Subscription subscription, decimal amount, DateTime issuedDate)
    {
        Id = Guid.NewGuid();
        SubscriptionId = subscription.Id;
        Amount = amount;
        IssuedDate = issuedDate;
        IsPaid = false;
    }

    internal void MarkAsPaid()
    {
        if (IsPaid)
            throw new BusinessLogicException("Invoice is already paid.");
        IsPaid = true;
        PaidDate = DateTime.UtcNow;
    }
}