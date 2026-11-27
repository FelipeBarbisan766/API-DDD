using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Infrastructure.Persistence.Configurations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API_DDD.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private const string GenericUniqueViolationMessage = "A record with the same unique value already exists.";

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            throw new ConflictException(BuildConflictMessage(exception));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        const int duplicateKeyIndex = 2601;
        const int duplicateKeyConstraint = 2627;

        return exception.InnerException is SqlException
        {
            Number: duplicateKeyIndex or duplicateKeyConstraint
        };
    }

    private static string BuildConflictMessage(DbUpdateException exception)
    {
        var violatedEmailIndex = exception.InnerException?.Message
            .Contains(UserConfiguration.EmailUniqueIndexName) == true;

        return violatedEmailIndex
            ? UserErrors.EmailAlreadyInUse
            : GenericUniqueViolationMessage;
    }
}