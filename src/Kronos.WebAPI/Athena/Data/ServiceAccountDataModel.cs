namespace Kronos.WebAPI.Athena.Data;

public class ServiceAccountDataModel 
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required byte[] Secret { get; set; }
    public ICollection<ServiceScopeDataModel> Scopes { get; set; } = new List<ServiceScopeDataModel>();
}
