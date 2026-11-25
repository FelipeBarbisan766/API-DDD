using API_DDD.Application.Services.Auth;

namespace API_DDD.Infrastructure.Security;

public interface ICookieAuthService
{
    Task SignInAsync(AuthenticatedUserDto user);
    Task SignOutAsync();
}