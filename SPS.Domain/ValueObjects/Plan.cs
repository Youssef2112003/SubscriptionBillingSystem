using SPS.Domain.Common;

namespace SPS.Domain.ValueObjects;

public class Plan(string name, decimal amount, BillingCycle cycle) : ValueObject
{
    public string Name { get; } = name;
    public decimal Amount { get; } = amount;
    public BillingCycle Cycle { get; } = cycle;

    protected override IEnumerable<object> GetObjectValues()
    {
        yield return Name;
        yield return Amount;
        yield return Cycle;
    }
}

public enum BillingCycle
{
    Monthly,
    Yearly
}