using Kronos.WebAPI.Hermes.WebApi;
using Kronos.WebAPI.Hermes.WebApi.Interop.Shared;

namespace Kronos.WebAPI.Hermes.Services;

public sealed class RegistrationSetup : Dictionary<CredentialsType, string>
{
    public bool IsEnabled(CredentialsType credentialsType) => this.ContainsKey(credentialsType) && this[credentialsType].Equals("enabled");
}
public sealed class HermesConfiguration
{
    public required JwtConfig Jwt { get; set; }
}