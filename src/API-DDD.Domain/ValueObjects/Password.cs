using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.Services;

namespace API_DDD.Domain.ValueObjects;

public sealed class Password : ValueObject
{
    private const int MinLength = 8;

    public string Hash { get; }

    private Password(string hash) => Hash = hash;

    public static Password Create(string plainPassword, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ValidationException("Password is required.");

        if (plainPassword.Length < MinLength)
            throw new ValidationException($"Password must have at least {MinLength} characters.");

        if (!plainPassword.Any(char.IsUpper) || !plainPassword.Any(char.IsDigit))
            throw new ValidationException("Password must contain an uppercase letter and a number.");

        return new Password(hasher.Hash(plainPassword));
    }

    public static Password FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ValidationException("Password hash is required.");

        return new Password(hash);
    }

    public bool Matches(string plainPassword, IPasswordHasher hasher)
        => hasher.Verify(plainPassword, Hash);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }
}