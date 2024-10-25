using System.ComponentModel.DataAnnotations;

namespace Kronos.WebAPI.Hermes.WebApi;

public sealed class JwtConfig
{
    [Required] public required string Issuer { get; set; }
    [Required] public required string Audience { get; set; }
    [Required] public required string Key { get; set; }
}