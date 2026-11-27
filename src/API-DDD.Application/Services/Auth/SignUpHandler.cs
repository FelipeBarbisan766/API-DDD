using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.Services;
using API_DDD.Domain.ValueObjects;

namespace API_DDD.Application.Services.Auth;

public sealed class SignUpHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public SignUpHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticatedUserDto> HandleAsync(
        SignUpCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        await EnsureEmailIsAvailableAsync(email, cancellationToken);

        var password = Password.Create(command.Password, _passwordHasher);
        var user = new User(command.Name, email, password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticatedUserDto(user.Id, user.Name, user.Email.Value);
    }

    private async Task EnsureEmailIsAvailableAsync(Email email, CancellationToken cancellationToken)
    {
        var emailAlreadyInUse = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (emailAlreadyInUse)
            throw new ConflictException(UserErrors.EmailAlreadyInUse);
    }
}