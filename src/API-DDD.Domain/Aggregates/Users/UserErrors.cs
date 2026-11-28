namespace API_DDD.Domain.Aggregates.Users;

public static class UserErrors
{
    public const string EmailAlreadyInUse = "Email is already in use.";
    public const string ProviderAlreadyLinked = "This user is already linked to another account of the same provider.";
}