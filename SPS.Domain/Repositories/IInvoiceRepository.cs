using SPS.Domain.Entities;

namespace SPS.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken);


}