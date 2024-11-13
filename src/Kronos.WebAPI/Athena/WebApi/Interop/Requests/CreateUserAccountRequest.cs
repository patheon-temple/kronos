namespace Kronos.WebAPI.Athena.WebApi.Interop.Requests;

internal class CreateUserAccountRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DeviceId { get; set; }
    public string[] Scopes { get; set; } = [];
}