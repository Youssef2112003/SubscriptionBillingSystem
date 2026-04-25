using SPS.Application.Common;
using SPS.Domain.Exceptions;
using SPS.Domain.Repositories;
using SPS.Domain.ValueObjects;

namespace SPS.Application.Subscriptions.Commands
{
    public class CreateSubscriptionHandler : ICommandHandler<CreateSubscriptionCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubscriptionHandler(
            ICustomerRepository customerRepository,
            ISubscriptionRepository subscriptionRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null) throw new BusinessLogicException("Customer not found.");

            var cycle = Enum.Parse<BillingCycle>(request.BillingCycle);
            var plan = new Plan(request.PlanName, request.Amount, cycle);
            var subscription = customer.AddSubscription(plan, request.StartDate);

            await _subscriptionRepository.AddAsync(subscription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Active SubscriptionActivated + InvoiceGenerated
            return subscription.Id;
        }
    }
}
