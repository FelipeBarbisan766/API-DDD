using API_DDD.Domain.ValueObjects;

namespace API_DDD.Domain.Aggregates.Users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByExternalLoginAsync(
        ExternalLoginProvider provider, string providerUserId, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}