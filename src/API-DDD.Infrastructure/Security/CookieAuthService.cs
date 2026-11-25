
using System.Security.Claims;
using API_DDD.Application.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace API_DDD.Infrastructure.Security;

public sealed class CookieAuthService : ICookieAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext =>
        _httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("No active HTTP context.");

    public async Task SignInAsync(AuthenticatedUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }

    public Task SignOutAsync() =>
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}