using API_DDD.Application.Services.Auth;
using API_DDD.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace API_DDD.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginHandler _loginHandler;
    private readonly ICookieAuthService _cookieAuthService;

    public AuthController(LoginHandler loginHandler, ICookieAuthService cookieAuthService)
    {
        _loginHandler = loginHandler;
        _cookieAuthService = cookieAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _loginHandler.HandleAsync(
            new LoginCommand(request.Email, request.Password), cancellationToken);

        await _cookieAuthService.SignInAsync(user);
        return NoContent();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _cookieAuthService.SignOutAsync();
        return NoContent();
    }
}

public sealed record LoginRequest(string Email, string Password);