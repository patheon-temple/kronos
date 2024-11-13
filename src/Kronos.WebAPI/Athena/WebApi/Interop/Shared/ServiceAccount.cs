namespace Kronos.WebAPI.Athena.WebApi.Interop.Shared;

internal class ServiceAccount
{
    public required string ServiceName { get; set; }
    public required Guid Id { get; set; }
    public string? AuthorizationCode { get; set; }
}