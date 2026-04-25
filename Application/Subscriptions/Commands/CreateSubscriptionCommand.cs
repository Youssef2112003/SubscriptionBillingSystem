using SPS.Application.Common;

namespace SPS.Application.Subscriptions.Commands
{
    public record CreateSubscriptionCommand(Guid CustomerId, string PlanName, decimal Amount, string BillingCycle, DateTime StartDate)
    : ICommand<Guid>;
}
