namespace Kronos.WebAPI.Athena.Data;

public class UserAccountDataModel
{
    public Guid UserId { get; set; }
    
    public string? DeviceId { get; set; }
    public string? Username { get; set; }
    public byte[]? PasswordHash { get; set; }
}