using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SPS.Application.Common;
using SPS.Domain.Repositories;

namespace SPS.Infrastructure.Jobs
{
    public class BillingCycleService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BillingCycleService> _logger;

        public BillingCycleService(IServiceScopeFactory scopeFactory, ILogger<BillingCycleService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var subscriptionRepo = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                var invoiceRepo = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

                var activeSubscriptions = await subscriptionRepo.GetActiveSubscriptionsAsync(stoppingToken);
                var now = dateTime.UtcNow;

                foreach (var sub in activeSubscriptions)
                {
                    if (sub.Plan == null)
                    {
                        _logger.LogWarning("Subscription {SubscriptionId} has no plan assigned. Skipping invoice generation.", sub.Id);
                        continue;
                    }

                    var lastInvoice = sub.Invoices.OrderByDescending(i => i.IssuedDate).FirstOrDefault();
                    var shouldGenerate = false;

                    if (lastInvoice == null) shouldGenerate = true;
                    else
                    {
                        var nextDate = sub.Plan.Cycle == Domain.ValueObjects.BillingCycle.Monthly
                            ? lastInvoice.IssuedDate.AddMonths(1)
                            : lastInvoice.IssuedDate.AddYears(1);
                        if (now >= nextDate) shouldGenerate = true;
                    }

                    if (shouldGenerate)
                    {
                        sub.GenerateInvoice(); // InvoiceGenerated event upload
                        await invoiceRepo.AddAsync(sub.Invoices.Last(), stoppingToken);
                    }
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Check every 5 minutes for daily productivity
            }
        }
    }
}
