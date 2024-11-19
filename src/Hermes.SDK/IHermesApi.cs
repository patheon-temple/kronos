using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public interface IHermesApi
    {
        Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string password, string[] requestedScopes, Guid audience,
            CancellationToken cancellationToken);

        Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password, string[] requestedScopes,
            Guid audience,
            CancellationToken cancellationToken);

        Task<TokenSet> CreateTokenSetForServiceAsync(Guid serviceId, string authorizationCode, string[] requestedScopes,
            Guid audience, CancellationToken cancellationToken = default);
    }
}