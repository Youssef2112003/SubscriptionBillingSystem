using Microsoft.EntityFrameworkCore;
using SPS.Domain.Entities;
using SPS.Domain.Repositories;

namespace SPS.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly Persistence.AppDbContext _context;

        public SubscriptionRepository(Persistence.AppDbContext context) => _context = context;

        public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Subscriptions
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Subscription>> GetActiveSubscriptionsAsync(CancellationToken cancellationToken = default)
            => await _context.Subscriptions
                .Where(s => s.IsActive)
                .Include(s => s.Invoices)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default)
            => await _context.Subscriptions.AddAsync(subscription, cancellationToken);

        public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default)
            => await Task.CompletedTask;

        public async Task<Subscription?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(s => s.Invoices.Any(i => i.Id == invoiceId), cancellationToken);
        }
    }
}
