namespace API_DDD.Application.Services.Auth;

public sealed record AuthenticatedUserDto(Guid Id, string Name, string Email);