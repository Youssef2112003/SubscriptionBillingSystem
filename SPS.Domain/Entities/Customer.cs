using SPS.Domain.Common;
using SPS.Domain.ValueObjects;

namespace SPS.Domain.Entities;

public class Customer : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    private readonly List<Subscription> _subscriptions = [];
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

    private Customer()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    public Customer(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public Subscription AddSubscription(Plan plan, DateTime startDate)
    {
        var subscription = new Subscription(this, plan, startDate);
        _subscriptions.Add(subscription);
        return subscription;
    }
}