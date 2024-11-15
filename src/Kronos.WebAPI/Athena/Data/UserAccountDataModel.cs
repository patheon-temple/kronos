namespace Kronos.WebAPI.Athena.Data;

public class UserAccountDataModel 
{
    public Guid Id { get; set; }
    public string? DeviceId { get; set; }
    public string? Username { get; set; }
    public byte[]? PasswordHash { get; set; }

    public ICollection<UserScopeDataModel> Scopes { get; set; } = new List<UserScopeDataModel>();
}
