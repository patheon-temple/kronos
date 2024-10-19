namespace Kronos.WebAPI.Athena.Domain;

public sealed class ServiceDescription{
    public required string Url { get; set; }
}
public sealed class ServiceDiscovery
{
    public required ServiceDescription Zeus { get; set; }
}