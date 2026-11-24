using API_DDD.Domain.SeedWork;

namespace API_DDD.Domain.Aggregates.Events;

public sealed record UserDeactivatedEvent(Guid UserId) : DomainEvent;