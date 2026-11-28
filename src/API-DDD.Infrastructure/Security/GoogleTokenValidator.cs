using API_DDD.Application.Services.Auth;
using API_DDD.Domain.Exceptions;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace API_DDD.Infrastructure.Security;

public sealed class GoogleTokenValidator : IGoogleTokenValidator
{
    private const string InvalidTokenMessage = "Invalid Google token.";

    private readonly GoogleAuthOptions _options;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GoogleIdentity> ValidateAsync(string idToken)
    {
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _options.ClientId }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

            return new GoogleIdentity(
                payload.Subject,
                payload.Email,
                payload.EmailVerified,
                payload.GivenName,
                payload.FamilyName);
        }
        catch (InvalidJwtException)
        {
            throw new ForbiddenException(InvalidTokenMessage);
        }
    }
}
