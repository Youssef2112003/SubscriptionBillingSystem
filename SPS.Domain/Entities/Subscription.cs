using SPS.Domain.Common;
using SPS.Domain.Events;
using SPS.Domain.Exceptions;
using SPS.Domain.ValueObjects;

namespace SPS.Domain.Entities;

public class Subscription : AggregateRoot<Guid>
{
    public Guid CustomerId { get; private set; }
    public Plan? Plan { get; private set; }
    public DateTime StartDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? CancelledDate { get; private set; }
    private readonly List<Invoice> _invoices = [];
    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();

    private Subscription() { }

    internal Subscription(Customer customer, Plan plan, DateTime startDate)
    {
        Id = Guid.NewGuid();
        CustomerId = customer.Id;
        Plan = plan;
        StartDate = startDate;
        IsActive = true;

        // First invoice upon activation
        if (plan is null)
            throw new ArgumentNullException(nameof(plan), "Plan cannot be null.");
        var invoice = new Invoice(this, plan.Amount, DateTime.UtcNow);
        _invoices.Add(invoice);

        // Activation event
        RaiseDomainEvent(new SubscriptionActivatedEvent(this.Id, customer.Id, invoice.Id));
        RaiseDomainEvent(new InvoiceGeneratedEvent(invoice.Id, this.Id));
    }

    public Invoice GenerateInvoice()
    {
        if (!IsActive)
            throw new BusinessLogicException("Cannot generate invoice for inactive subscription.");
        if (Plan is null)
            throw new BusinessLogicException("Cannot generate invoice without a plan.");

        var invoice = new Invoice(this, Plan.Amount, DateTime.UtcNow);
        _invoices.Add(invoice);
        RaiseDomainEvent(new InvoiceGeneratedEvent(invoice.Id, this.Id));
        return invoice;
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new BusinessLogicException("Subscription is already cancelled.");
        IsActive = false;
        CancelledDate = DateTime.UtcNow;
    }

    public void PayInvoice(Guid invoiceId)
    {
        var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId)
                      ?? throw new BusinessLogicException("Invoice not found.");
        invoice.MarkAsPaid();
        RaiseDomainEvent(new PaymentReceivedEvent(invoice.Id, this.Id));
    }
}