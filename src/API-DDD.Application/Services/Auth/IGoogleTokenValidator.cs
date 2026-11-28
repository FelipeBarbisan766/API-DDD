namespace API_DDD.Application.Services.Auth;

public interface IGoogleTokenValidator
{
    Task<GoogleIdentity> ValidateAsync(string idToken);
}
