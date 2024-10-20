namespace Kronos.WebAPI.Athena.WebApi.Interop.Responses;

public sealed class PantheonIdentityResponse
{
    public required Guid Id { get; set; }
    public string? DeviceId { get; set; }
}