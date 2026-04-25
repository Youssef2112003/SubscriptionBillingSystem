using MediatR;
using SPS.Application.Common;
using SPS.Domain.Exceptions;
using SPS.Domain.Repositories;

namespace SPS.Application.Subscriptions.Commands
{
    public class CancelSubscriptionHandler : ICommandHandler<CancelSubscriptionCommand, Unit>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelSubscriptionHandler(ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);
            if (subscription == null) throw new BusinessLogicException("Subscription not found.");

            subscription.Cancel();
            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
