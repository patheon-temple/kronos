namespace Kronos.WebAPI.Athena.WebApi.Interop.Responses;

public sealed class CreateDeviceIdentityResponse
{
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Unique device ID
    /// </summary>
    /// <example>00-B0-D0-63-C2-26</example>
    public string? DeviceId { get; set; }
}