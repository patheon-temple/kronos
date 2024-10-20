namespace Kronos.WebAPI.Kronos.Domain;

public sealed class ServiceDescription
{
    public required string Url { get; set; }
    public required string Description { get; set; }
}

public sealed class ServiceDiscovery : Dictionary<string, ServiceDescription>
{
}