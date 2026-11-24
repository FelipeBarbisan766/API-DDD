namespace API_DDD.Domain.SeedWork;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
    bool IsDeleted => DeletedAt.HasValue;
}