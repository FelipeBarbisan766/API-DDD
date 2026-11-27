namespace API_DDD.Controllers.Requests;

public sealed record SignUpRequest(string Name, string Email, string Password);
