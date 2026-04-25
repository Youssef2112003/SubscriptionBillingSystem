using Microsoft.EntityFrameworkCore;
using SPS.Domain.Entities;
using SPS.Domain.Repositories;

namespace SPS.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly Persistence.AppDbContext _context;

        public InvoiceRepository(Persistence.AppDbContext context) => _context = context;

        public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Invoice>> GetBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
            => await _context.Invoices.Where(i => i.SubscriptionId == subscriptionId).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Invoices.ToListAsync(cancellationToken);

        public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
            => await _context.Invoices.AddAsync(invoice, cancellationToken);

        public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
            => await Task.CompletedTask;


    }
}
