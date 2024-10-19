namespace Zeus.Interop.Requests;

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
    /// <example>45dd14cd-cba8-4657-a170-d59e2fc9696c</example>
    public  string? DeviceId { get; set; }
}