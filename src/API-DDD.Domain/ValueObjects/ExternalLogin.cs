using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;

namespace API_DDD.Domain.ValueObjects;

public sealed class ExternalLogin : ValueObject
{
    public ExternalLoginProvider Provider { get; }
    public string ProviderUserId { get; }
    public DateTime LinkedAt { get; }

    private ExternalLogin(ExternalLoginProvider provider, string providerUserId, DateTime linkedAt)
    {
        Provider = provider;
        ProviderUserId = providerUserId;
        LinkedAt = linkedAt;
    }

    public static ExternalLogin Create(ExternalLoginProvider provider, string providerUserId)
    {
        if (!Enum.IsDefined(provider))
            throw new ValidationException("External login provider is invalid.");

        if (string.IsNullOrWhiteSpace(providerUserId))
            throw new ValidationException("External login user id is required.");

        return new ExternalLogin(provider, providerUserId.Trim(), DateTime.UtcNow);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Provider;
        yield return ProviderUserId;
    }
}
