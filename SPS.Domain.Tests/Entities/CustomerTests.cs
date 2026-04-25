using SPS.Domain.Entities;
using SPS.Domain.ValueObjects;
using Xunit;

namespace SPS.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void CreateCustomer_Valid_ShouldAssignId()
    {
        var customer = new Customer("Youssef", "youssef@gmail.com");
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("Youssef", customer.Name);
        Assert.Equal("youssef@gmail.com", customer.Email);
    }

    [Fact]
    public void AddSubscription_ShouldCreateActiveSubscriptionWithInvoice()
    {
        var customer = new Customer("Sana", "Sana@gmail.com");
        var plan = new Plan("Basic", 10m, BillingCycle.Monthly);
        var subscription = customer.AddSubscription(plan, DateTime.UtcNow);

        Assert.True(subscription.IsActive);
        Assert.Single(subscription.Invoices);
        Assert.Equal(10m, subscription.Invoices.First().Amount);
    }
}