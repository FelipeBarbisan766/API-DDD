using API_DDD.Domain.Aggregates.Events;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.Services;
using API_DDD.Domain.ValueObjects;

namespace API_DDD.Domain.Aggregates.Users;

public class User : AggregateRoot, IAuditable, ISoftDeletable
{
    private readonly List<ExternalLogin> _externalLogins = new();

    public string Name { get; private set; } = null!;
    public string? Surname { get; private set; }
    public Email Email { get; private set; } = null!;
    public Password? Password { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? EmailConfirmedAt { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<ExternalLogin> ExternalLogins => _externalLogins.AsReadOnly();

    public bool IsEmailConfirmed => EmailConfirmedAt is not null;
    public bool HasPassword => Password is not null;

    private User() { }

    public User(string name, Email email, Password password, string? surname = null)
        : this(name, email, surname)
    {
        if (password is null)
            throw new ValidationException("Password is required.");

        Password = password;
    }

    private User(string name, Email email, string? surname)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");
        if (email is null)
            throw new ValidationException("Email is required.");

        Name = name.Trim();
        Surname = NormalizeSurname(surname);
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserCreatedEvent(Id, Name, email.Value));
    }
    
    public static User CreateFromVerifiedExternalLogin(
        string name, string? surname, Email email, ExternalLogin externalLogin)
    {
        if (externalLogin is null)
            throw new ValidationException("External login is required.");

        var user = new User(name, email, surname);
        user._externalLogins.Add(externalLogin);
        user.EmailConfirmedAt = DateTime.UtcNow;

        return user;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Name is required.");

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSurname(string? newSurname)
    {
        Surname = NormalizeSurname(newSurname);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeEmail(Email newEmail)
    {
        if (newEmail is null)
            throw new ValidationException("Email is required.");

        Email = newEmail;
        EmailConfirmedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(Password newPassword)
    {
        if (newPassword is null)
            throw new ValidationException("Password is required.");

        Password = newPassword;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerifyPassword(string plainPassword, IPasswordHasher hasher)
    {
        return Password is not null && Password.Matches(plainPassword, hasher);
    }
    
    public void LinkVerifiedExternalLogin(ExternalLogin externalLogin)
    {
        if (externalLogin is null)
            throw new ValidationException("External login is required.");

        var currentLink = _externalLogins.FirstOrDefault(link => link.Provider == externalLogin.Provider);

        if (currentLink is not null)
        {
            if (currentLink.Equals(externalLogin)) return;

            throw new ConflictException(UserErrors.ProviderAlreadyLinked);
        }

        var wasEmailConfirmed = IsEmailConfirmed;

        _externalLogins.Add(externalLogin);

        if (!wasEmailConfirmed)
            Password = null;

        ConfirmEmail();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        if (IsEmailConfirmed) return;

        EmailConfirmedAt = DateTime.UtcNow;
        UpdatedAt = EmailConfirmedAt;
    }

    public void Enable()
    {
        if (IsActive) return;
        IsActive = true;
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    public void Disable()
    {
        if (!IsActive) return;
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void EnsureCanLogin()
    {
        if (DeletedAt is not null || !IsActive)
            throw new ForbiddenException("Account is disabled.");
    }

    private static string? NormalizeSurname(string? surname)
    {
        return string.IsNullOrWhiteSpace(surname) ? null : surname.Trim();
    }
}