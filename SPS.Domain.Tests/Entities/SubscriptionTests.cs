using SPS.Domain.Entities;
using SPS.Domain.Exceptions;
using SPS.Domain.ValueObjects;
using Xunit;

namespace SPS.Domain.Tests.Entities;

public class SubscriptionTests
{
    [Fact]
    public void GenerateInvoice_WhenActive_ShouldCreateInvoice()
    {
        var customer = new Customer("Test", "test@test.com");
        var plan = new Plan("Pro", 20m, BillingCycle.Yearly);
        var sub = customer.AddSubscription(plan, DateTime.UtcNow);
        int initialCount = sub.Invoices.Count;

        sub.GenerateInvoice();
        Assert.Equal(initialCount + 1, sub.Invoices.Count);
        Assert.Equal(20m, sub.Invoices.Last().Amount);
    }

    [Fact]
    public void GenerateInvoice_WhenCancelled_ShouldThrow()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Pro", 20m, BillingCycle.Monthly), DateTime.UtcNow);
        sub.Cancel();

        Assert.Throws<BusinessLogicException>(() => sub.GenerateInvoice());
    }

    [Fact]
    public void PayInvoice_WhenNotPaid_ShouldMarkAsPaid()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        var invoice = sub.Invoices.First();

        sub.PayInvoice(invoice.Id);
        Assert.True(invoice.IsPaid);
        Assert.NotNull(invoice.PaidDate);
    }

    [Fact]
    public void PayInvoice_WhenAlreadyPaid_ShouldThrow()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        var invoice = sub.Invoices.First();
        sub.PayInvoice(invoice.Id);

        Assert.Throws<BusinessLogicException>(() => sub.PayInvoice(invoice.Id));
    }

    [Fact]
    public void Cancel_WhenActive_ShouldSetIsActiveFalse()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        sub.Cancel();
        Assert.False(sub.IsActive);
        Assert.NotNull(sub.CancelledDate);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrow()
    {
        var customer = new Customer("Test", "test@test.com");
        var sub = customer.AddSubscription(new Plan("Basic", 5m, BillingCycle.Monthly), DateTime.UtcNow);
        sub.Cancel();
        Assert.Throws<BusinessLogicException>(() => sub.Cancel());
    }
}