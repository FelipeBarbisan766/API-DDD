namespace API_DDD.Application.Services.Auth;

public sealed record SignUpCommand(string Name, string Email, string Password);
