using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Hermes.SDK;

public interface IHermesApi
{
    Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, CancellationToken cancellationToken);

    Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password,
        CancellationToken cancellationToken);
}