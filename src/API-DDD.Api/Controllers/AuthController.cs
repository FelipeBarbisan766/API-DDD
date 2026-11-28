using API_DDD.Application.Services.Auth;
using API_DDD.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace API_DDD.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginHandler _loginHandler;
    private readonly GoogleLoginHandler _googleLoginHandler;
    private readonly ICookieAuthService _cookieAuthService;

    public AuthController(
        LoginHandler loginHandler,
        GoogleLoginHandler googleLoginHandler,
        ICookieAuthService cookieAuthService)
    {
        _loginHandler = loginHandler;
        _googleLoginHandler = googleLoginHandler;
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

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _googleLoginHandler.HandleAsync(
            new GoogleLoginCommand(request.IdToken), cancellationToken);

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

public sealed record GoogleLoginRequest(string IdToken);