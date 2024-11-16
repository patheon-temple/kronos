namespace Kronos.WebAPI;

public static class GlobalDefinitions
{
    public static class ClaimTypes
    {
        public const string AccountType = "actp";
    }

    internal static class Scopes
    {
        public const string Superuser = "superuser";
    }

    public static class Policies
    {
        public const string SuperUser = "superuser";
    }

    public static class Jwt
    {
        public static Guid AthenaAudience { get; } = Guid.Parse("1c95fa3e-bcae-4ecb-a585-7fca315cc377");
        public const string Issuer = "hermes";
    }

    public enum AccountType
    {
        User = 0,
        Service = 1
    }

    public static class Limits
    {
        public const int EncryptionKeyMaxSize = 32;
        public const int SigningKeyMaxSize = 512;
    }

    public static class ConfigurationKeys
    {
        public const string PostgresConnectionString = "Postgres";
        public const string HermesConfiguration = "HermesConfiguration:Jwt";
    }

    public static class Headers
    {
        public const string ValidateOnly = "X-Pantheon-Validate-Only";
    }
    
}