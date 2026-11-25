using System.Net.Mail;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;

namespace API_DDD.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 254; 

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            throw new ValidationException("Email is required.");

        var normalizedValue = rawValue.Trim().ToLowerInvariant();

        if (normalizedValue.Length > MaxLength)
            throw new ValidationException($"Email must have at most {MaxLength} characters.");

        if (!IsValidFormat(normalizedValue))
            throw new ValidationException("Email format is invalid.");

        return new Email(normalizedValue);
    }

    private static bool IsValidFormat(string value)
    {
        return MailAddress.TryCreate(value, out var address)
               && address.Address == value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}