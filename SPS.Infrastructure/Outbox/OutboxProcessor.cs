using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SPS.Domain.Common;
using System.Text.Json;

namespace SPS.Infrastructure.Outbox;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Persistence.AppDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var messages = await db.OutboxMessages
                .Where(m => m.ProcessedOn == null)
                .OrderBy(m => m.OccurredOn)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    var type = Type.GetType($"SPS.Domain.Events.{message.Type}, SPS.Domain");
                    if (type == null) continue;
                    var domainEvent = JsonSerializer.Deserialize(message.Data, type) as IDomainEvent;
                    if (domainEvent != null)
                    {
                        await mediator.Publish(domainEvent, stoppingToken);
                    }
                    message.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                }
            }

            if (messages.Any())
                await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}