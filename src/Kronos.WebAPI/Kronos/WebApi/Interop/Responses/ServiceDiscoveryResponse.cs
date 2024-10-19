namespace Kronos.WebAPI.Kronos.WebApi.Interop.Responses;

public sealed class ServiceDescriptionResponse
{
    public required string Url { get; set; }
}
public sealed class ServiceDiscoveryResponse
{
    public required ServiceDescriptionResponse Zeus { get; set; }
}