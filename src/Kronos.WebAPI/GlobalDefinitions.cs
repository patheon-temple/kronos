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
}