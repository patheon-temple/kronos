using Kronos.WebAPI.Hermes.WebApi.Interop.Shared;

namespace Kronos.WebAPI.Hermes.WebApi.Interop.Requests;

public class AuthenticationPostRequest
{
    public AuthenticationPostRequest(string? username)
    {
        Username = username;
    }

    public CredentialsType CredentialsType { get; set; } = CredentialsType.DeviceId;
    public string[] RequestedScopes { get; set; } = [];
    public string? Password { get; set; }
    public string? DeviceId { get; set; }
    public string? Username { get; set; }
    public Guid? ServiceId { get; set; }
    public string? AuthorizationCode { get; set; }
}