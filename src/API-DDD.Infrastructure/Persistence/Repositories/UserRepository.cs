using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace API_DDD.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByExternalLoginAsync(
        ExternalLoginProvider provider, string providerUserId, CancellationToken cancellationToken)
    {
        return _context.Users.FirstOrDefaultAsync(
            user => user.ExternalLogins.Any(login =>
                login.Provider == provider && login.ProviderUserId == providerUserId),
            cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return _context.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
}