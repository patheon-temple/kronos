namespace Kronos.WebAPI.Athena.Domain;

public sealed class AthenaConfiguration
{
    public required string SuperuserUsername { get; set; }
    public required string SuperuserPassword { get; set; }
    public required Guid SuperuserId { get; set; }
    
}