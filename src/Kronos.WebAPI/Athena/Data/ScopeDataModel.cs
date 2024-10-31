namespace Kronos.WebAPI.Athena.Data;

public class ScopeDataModel
{
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public ICollection<UserAccountDataModel> UserAccounts { get; set; } = new List<UserAccountDataModel>();
}