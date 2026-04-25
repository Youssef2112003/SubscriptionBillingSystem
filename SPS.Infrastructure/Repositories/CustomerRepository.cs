using Microsoft.EntityFrameworkCore;
using SPS.Domain.Entities;
using SPS.Domain.Repositories;

namespace SPS.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Persistence.AppDbContext _context;

        public CustomerRepository(Persistence.AppDbContext context) => _context = context;

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
            => await _context.Customers.AddAsync(customer, cancellationToken);
    }
}
