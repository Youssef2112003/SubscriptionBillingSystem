using FluentValidation;
using SPS.Domain.ValueObjects;

namespace SPS.Application.Subscriptions.Commands
{
    public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        public CreateSubscriptionValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.PlanName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.BillingCycle).Must(x => Enum.TryParse<BillingCycle>(x, out _)).WithMessage("Invalid billing cycle");
            RuleFor(x => x.StartDate).GreaterThan(DateTime.MinValue);
        }
    }
}
