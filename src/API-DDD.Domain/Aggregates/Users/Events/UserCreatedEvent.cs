using API_DDD.Domain.SeedWork;

namespace API_DDD.Domain.Aggregates.Events;

public sealed record UserCreatedEvent(Guid UserId, string Name, string Email) : DomainEvent;