namespace Kronos.WebAPI.Athena.Data;

public class UserAccountScopeDataModel
{
    public required UserAccountDataModel Account { get; set; }
    public required UserScopeDataModel Scope { get; set; }
}

public class ServiceAccountScopeDataModel
{
    public required ServiceAccountDataModel Account { get; set; }
    public required ServiceScopeDataModel Scope { get; set; }
}