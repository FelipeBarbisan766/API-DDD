using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.Services;
using API_DDD.Domain.ValueObjects;

namespace API_DDD.Application.Services.Auth;


public sealed class LoginHandler
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticatedUserDto> HandleAsync(
        LoginCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Password))
            throw new ForbiddenException(InvalidCredentialsMessage);

        Email email;
        try
        {
            email = Email.Create(command.Email);
        }
        catch (ValidationException)
        {
            throw new ForbiddenException(InvalidCredentialsMessage);
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.VerifyPassword(command.Password, _passwordHasher))
            throw new ForbiddenException(InvalidCredentialsMessage);

        user.EnsureCanLogin();

        return new AuthenticatedUserDto(user.Id, user.Name, user.Email.Value);
    }
}