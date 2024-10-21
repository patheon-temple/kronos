using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Hermes.SDK;

public sealed class TokenSet
{
    public required string AccessToken { get; set; }
}

public interface IHermesApi
{
    Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, CancellationToken cancellationToken);
}