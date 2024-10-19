namespace Kronos.WebAPI.Kronos.Domain;

public sealed class ServiceDescription
{
    public required string Url { get; set; }
    public required string Description { get; set; }
}

public sealed class ServiceDiscovery
{
    public ServiceDescription Athena { get; } = new()
    {
        Url = "",
        Description = "Identities API"
    };

    public ServiceDescription Hermes { get; } = new()
    {
        Url = "",
        Description = "Token API"
    };
}