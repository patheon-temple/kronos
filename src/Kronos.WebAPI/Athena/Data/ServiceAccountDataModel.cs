namespace Kronos.WebAPI.Athena.Data;

public class ServiceAccountDataModel
{
    public Guid ServiceId { get; set; }
    public required byte[] Secret { get; set; }
}