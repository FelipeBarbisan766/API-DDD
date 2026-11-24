using API_DDD.Domain.Aggregates.Events;
using API_DDD.Domain.SeedWork;

namespace API_DDD.Domain.Aggregates.Users;

public class User : AggregateRoot, IAuditable, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public User(string name, Email email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
        if (email is null)
            throw new DomainException("E-mail é obrigatório.");

        Name = name;
        Email = email;
        IsActive = true;

        AddDomainEvent(new UserCreatedEvent(Id, name, email.Valor));
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Nome é obrigatório.");

        Name = newName;
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