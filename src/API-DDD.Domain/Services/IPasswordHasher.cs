namespace API_DDD.Domain.Services;

public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string hash);
}