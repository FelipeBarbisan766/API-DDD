using API_DDD.Domain.Aggregates;
using API_DDD.Domain.Aggregates.Users;
using API_DDD.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_DDD.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .IsRequired()
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
                password => password.Hash,
                hash => Password.FromHash(hash))
            .HasColumnName("PasswordHash")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
    }
}