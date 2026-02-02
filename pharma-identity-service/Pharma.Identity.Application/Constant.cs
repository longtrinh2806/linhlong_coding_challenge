namespace Pharma.Identity.Application;

public static class Constant
{
    public static class CacheKeys
    {
        public static string PendingUser(string email) => $"pending-user:{email}";
        public static string RefreshToken(Ulid userId) => $"refresh-token:{userId}";
    }

    public static class CacheExpiration
    {
        public const int PendingUserMinutes = 5;
        public const int EmailOtpMinutes = 1;
    }
    
    public const string IdentityDatabaseSchema = "pharma_identity";

    public const string ApplicationName = "PharmaApp";
}