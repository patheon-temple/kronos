namespace Kronos.WebAPI.Athena.Data;

public class UserAccountDataModel
{
    public Guid UserId { get; set; }

    public string? DeviceId { get; set; }
    public string? Username { get; set; }
    public byte[]? PasswordHash { get; set; }

    public ICollection<ScopeDataModel> Scopes { get; set; } = new List<ScopeDataModel>();
}

public class UserScopeDataModel
{
    public UserAccountDataModel UserAccount { get; set; }
    public ScopeDataModel Scope { get; set; }
}