using MediatR;

namespace SPS.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}