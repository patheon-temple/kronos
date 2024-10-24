namespace Kronos.WebAPI.Hermes.WebApi.Interop.Requests;

public sealed class AuthenticationPostRequest
{
    /// <summary>
    /// Type of credentials
    /// </summary>
    /// <example>1</example>
    public required CredentialsType CredentialsType { get; set; } = CredentialsType.DeviceId;

    /// <summary>
    /// Unique device ID
    /// </summary>
    /// <example>00-B0-D0-63-C2-26</example>
    public string? DeviceId { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }
}