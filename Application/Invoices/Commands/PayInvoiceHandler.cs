using MediatR;
using SPS.Application.Common;
using SPS.Domain.Exceptions;
using SPS.Domain.Repositories;

namespace SPS.Application.Invoices.Commands
{
    public class PayInvoiceHandler : ICommandHandler<PayInvoiceCommand, Unit>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PayInvoiceHandler(ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
        {
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
        {
            // To recover the Aggregate, we must bring the subscription that owns the invoice.
            // We will need a way to retrieve the subscription via the invoice ID.
            // We will add a function in ISubscriptionRepository for that.
            var subscription = await _subscriptionRepository.GetByInvoiceIdAsync(request.InvoiceId, cancellationToken);
            if (subscription == null)
                throw new BusinessLogicException("Subscription not found for this invoice.");

            subscription.PayInvoice(request.InvoiceId);

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.SaveEntitiesAsync(cancellationToken); // Publish Domain Events

            return Unit.Value;
        }
    }
}
