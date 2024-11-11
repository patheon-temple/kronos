namespace Kronos.WebAPI.Athena.WebApi.Interop.Responses;

public class PantheonIdentity(global::Athena.SDK.Models.PantheonIdentity pantheonIdentity)
{
    public Guid Id { get; init; } = pantheonIdentity.Id;
    public string? DeviceId { get; init; } = pantheonIdentity.DeviceId;
    public string? Username { get; init; } = pantheonIdentity.Username;
}