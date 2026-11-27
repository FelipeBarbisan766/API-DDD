using API_DDD.Domain.Services;

namespace API_DDD.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
    }

    public bool Verify(string plainPassword, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
    }
}
