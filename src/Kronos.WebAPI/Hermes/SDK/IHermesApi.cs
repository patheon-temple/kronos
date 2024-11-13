namespace Kronos.WebAPI.Hermes.SDK;

public interface IHermesApi
{
    Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string[] requestedScopes, CancellationToken cancellationToken);

    Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password, string[] requestedScopes,
        CancellationToken cancellationToken);
    
    Task<TokenSet> CreateTokenSetForServiceAsync(Guid serviceId, byte[] secret, string[] requestedScopes, CancellationToken cancellationToken= default);

}