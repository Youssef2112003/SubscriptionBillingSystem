using MediatR;
using SPS.Application.Common;

namespace SPS.Application.Subscriptions.Commands
{
    public record CancelSubscriptionCommand(Guid SubscriptionId) : ICommand<Unit>;
}
