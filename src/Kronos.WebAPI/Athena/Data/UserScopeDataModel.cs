namespace Kronos.WebAPI.Athena.Data;

public class UserScopeDataModel
{
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public ICollection<UserAccountDataModel> Accounts { get; set; } = new List<UserAccountDataModel>();
}

public class ServiceScopeDataModel
{
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public ICollection<ServiceAccountDataModel> Accounts { get; set; } = new List<ServiceAccountDataModel>();
}