using API_DDD.Domain.Aggregates.Events;
using API_DDD.Domain.Exceptions;
using API_DDD.Domain.SeedWork;
using API_DDD.Domain.Services;
using API_DDD.Domain.ValueObjects;

namespace API_DDD.Domain.Aggregates.Users;

public class User : AggregateRoot, IAuditable, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime? EmailConfirmedAt { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsEmailConfirmed => EmailConfirmedAt is not null;

    private User() { }

    public User(string name, Email email, Password password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is required.");
        if (email is null)
            throw new ValidationException("Email is required.");
        if (password is null)
            throw new ValidationException("Password is required.");

        Name = name.Trim();
        Email = email;
        Password = password;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserCreatedEvent(Id, Name, email.Value));
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Name is required.");

        Name = newName.Trim();
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
        return Password.Matches(plainPassword, hasher);
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
}