using MediatR;
using Microsoft.EntityFrameworkCore;
using SPS.Application.Common;
using SPS.Domain.Common;
using SPS.Domain.Entities;
using SPS.Infrastructure.Outbox;

namespace SPS.Infrastructure.Persistence
{

    public class AppDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        private readonly IDateTime _dateTime;

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator, IDateTime dateTime)
            : base(options)
        {
            _mediator = mediator;
            _dateTime = dateTime;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).HasMaxLength(100);
                entity.Property(c => c.Email).HasMaxLength(200);
                entity.Ignore(c => c.DomainEvents);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.OwnsOne(s => s.Plan, plan =>
                {
                    plan.Property(p => p.Name).HasColumnName("PlanName");
                    plan.Property(p => p.Amount).HasColumnName("Amount");
                    plan.Property(p => p.Cycle).HasColumnName("BillingCycle");
                });
                entity.Ignore(s => s.DomainEvents);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Amount).HasColumnType("decimal(18,2)");
                entity.Ignore(i => i.DomainEvents);
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Type).IsRequired();
                entity.Property(o => o.Data).IsRequired();
            });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var aggregateRoots = ChangeTracker.Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(a => a.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = aggregateRoots.SelectMany(a => a.DomainEvents).ToList();

            foreach (var root in aggregateRoots)
                root.ClearDomainEvents();

            // Storing events in Outbox
            foreach (var domainEvent in domainEvents)
            {
                var outboxMessage = new OutboxMessage(
                    domainEvent.GetType().Name,
                    System.Text.Json.JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    _dateTime.UtcNow);
                OutboxMessages.Add(outboxMessage);
            }

            var result = await SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }

}