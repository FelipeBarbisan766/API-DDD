namespace API_DDD.Application.Services.Auth;


public sealed record LoginCommand(string Email, string Password);