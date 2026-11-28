namespace API_DDD.Application.Services.Auth;

public sealed record GoogleIdentity(
    string Subject,
    string Email,
    bool IsEmailVerified,
    string? GivenName,
    string? FamilyName);
