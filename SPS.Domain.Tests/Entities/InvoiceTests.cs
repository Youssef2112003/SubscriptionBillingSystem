using SPS.Domain.Entities;
using SPS.Domain.Exceptions;
using SPS.Domain.ValueObjects;
using Xunit;

namespace SPS.Domain.Tests.Entities;

public class InvoiceTests
{
    [Fact]
    public void MarkAsPaid_ShouldSetIsPaidTrue()
    {
        // Invoice cannot be created directly (internal constructor), so we create it via Subscription
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        var invoice = sub.Invoices.First();

        Assert.False(invoice.IsPaid);
        sub.PayInvoice(invoice.Id);
        Assert.True(invoice.IsPaid);
    }

    [Fact]
    public void MarkAsPaid_WhenAlreadyPaid_ShouldThrow()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        var invoice = sub.Invoices.First();
        sub.PayInvoice(invoice.Id);

        var markAsPaidMethod = typeof(Invoice).GetMethod("MarkAsPaid", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.NotNull(markAsPaidMethod);

        Assert.Throws<BusinessLogicException>(() => markAsPaidMethod.Invoke(invoice, null));

        // the call must be made via PayInvoice, not directly; this is a test for MarkAsPaid
    }
}