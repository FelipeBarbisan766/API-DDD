using API_DDD.Application.Services.Auth;
using API_DDD.Controllers.Requests;
using API_DDD.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace API_DDD.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class SignUpController : ControllerBase
{
    private readonly SignUpHandler _signUpHandler;
    private readonly ICookieAuthService _cookieAuthService;

    public SignUpController(SignUpHandler signUpHandler, ICookieAuthService cookieAuthService)
    {
        _signUpHandler = signUpHandler;
        _cookieAuthService = cookieAuthService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(
        [FromBody] SignUpRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SignUpCommand(request.Name, request.Email, request.Password);

        var authenticatedUser = await _signUpHandler.HandleAsync(command, cancellationToken);

        await _cookieAuthService.SignInAsync(authenticatedUser);

        return StatusCode(StatusCodes.Status201Created, authenticatedUser);
    }
}
