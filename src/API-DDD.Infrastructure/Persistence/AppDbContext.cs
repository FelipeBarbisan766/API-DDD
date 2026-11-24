using API_DDD.Domain.SeedWork;

namespace API_DDD.Infrastructure.Persistence;

public class AppDbContext
{
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Any())
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();

        var result = await base.SaveChangesAsync(ct);

        foreach (var e in events)
            await _publisher.Publish(e, ct); // ex.: MediatR IPublisher

        aggregates.ForEach(a => a.ClearDomainEvents());

        return result;
    }
}