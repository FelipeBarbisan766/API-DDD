using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.ValueObjects;

namespace API_DDD.Application.Services.Auth;

public sealed class GoogleLoginHandler
{
    private const string UnverifiedEmailMessage = "Google account email is not verified.";
    private const char EmailSeparator = '@';

    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GoogleLoginHandler(
        IGoogleTokenValidator googleTokenValidator,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _googleTokenValidator = googleTokenValidator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticatedUserDto> HandleAsync(
        GoogleLoginCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.IdToken))
            throw new ValidationException("Google ID token is required.");

        var identity = await _googleTokenValidator.ValidateAsync(command.IdToken);

        if (!identity.IsEmailVerified)
            throw new ForbiddenException(UnverifiedEmailMessage);

        var email = Email.Create(identity.Email);
        var externalLogin = ExternalLogin.Create(ExternalLoginProvider.Google, identity.Subject);

        var user = await FindLinkedUserAsync(externalLogin, cancellationToken)
                   ?? await LinkToExistingUserAsync(email, externalLogin, cancellationToken)
                   ?? await CreateUserAsync(identity, email, externalLogin, cancellationToken);

        user.EnsureCanLogin();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticatedUserDto(user.Id, user.Name, user.Email.Value);
    }

    private Task<User?> FindLinkedUserAsync(ExternalLogin externalLogin, CancellationToken cancellationToken)
    {
        return _userRepository.GetByExternalLoginAsync(
            externalLogin.Provider, externalLogin.ProviderUserId, cancellationToken);
    }

    private async Task<User?> LinkToExistingUserAsync(
        Email email, ExternalLogin externalLogin, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

        existingUser?.LinkVerifiedExternalLogin(externalLogin);

        return existingUser;
    }

    private async Task<User> CreateUserAsync(
        GoogleIdentity identity, Email email, ExternalLogin externalLogin, CancellationToken cancellationToken)
    {
        var name = string.IsNullOrWhiteSpace(identity.GivenName)
            ? email.Value.Split(EmailSeparator)[0]
            : identity.GivenName;

        var user = User.CreateFromVerifiedExternalLogin(name, identity.FamilyName, email, externalLogin);

        await _userRepository.AddAsync(user, cancellationToken);

        return user;
    }
}