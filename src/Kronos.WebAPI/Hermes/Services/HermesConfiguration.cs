using Kronos.WebAPI.Hermes.WebApi;

namespace Kronos.WebAPI.Hermes.Services;

public sealed class HermesConfiguration
{
    public required JwtConfig Jwt { get; set; }
}