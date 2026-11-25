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
    public bool IsActive { get; private set; }

    private User() { }

    public User(string name, Email email, Password password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Nome é obrigatório.");
        if (email is null)
            throw new ValidationException("E-mail é obrigatório.");

        Name = name;
        Email = email;
        Password = password;
        IsActive = true;

        AddDomainEvent(new UserCreatedEvent(Id, name, email.Value));
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ValidationException("Nome é obrigatório.");

        Name = newName;
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerifyPassword(string plainPassword, IPasswordHasher hasher)
    {
        return Password.Matches(plainPassword, hasher);
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
}