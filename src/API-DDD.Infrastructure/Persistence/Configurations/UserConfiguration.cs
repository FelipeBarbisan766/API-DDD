using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_DDD.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const string EmailUniqueIndexName = "IX_Users_Email";
    public const string ExternalLoginUniqueIndexName = "IX_UserExternalLogins_Provider_ProviderUserId";

    private const string ExternalLoginOwnerKey = "UserId";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(user => user.Surname)
            .IsRequired(false)
            .HasMaxLength(150);

        builder.Property(user => user.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasColumnName("Email")
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(user => user.Password)
            .HasConversion(
                password => password!.Hash,
                hash => Password.FromHash(hash))
            .HasColumnName("PasswordHash")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName(EmailUniqueIndexName);

        ConfigureExternalLogins(builder);
    }

    private static void ConfigureExternalLogins(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(user => user.ExternalLogins, externalLogins =>
        {
            externalLogins.ToTable("UserExternalLogins");

            externalLogins.WithOwner().HasForeignKey(ExternalLoginOwnerKey);
            externalLogins.HasKey(ExternalLoginOwnerKey, nameof(ExternalLogin.Provider));

            externalLogins.Property(login => login.Provider)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            externalLogins.Property(login => login.ProviderUserId)
                .HasMaxLength(255)
                .IsRequired();

            externalLogins.Property(login => login.LinkedAt)
                .IsRequired();

            externalLogins.HasIndex(login => new { login.Provider, login.ProviderUserId })
                .IsUnique()
                .HasDatabaseName(ExternalLoginUniqueIndexName);
        });

        builder.Navigation(user => user.ExternalLogins)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}