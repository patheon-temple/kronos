using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Hermes.SDK;

public sealed class TokenSet
{
    public required string AccessToken { get; set; }
}

public interface IHermesApi
{
    TokenSet CreateTokenSetForIdentity(PantheonIdentity identity);
}