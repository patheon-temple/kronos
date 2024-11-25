using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public interface IHermesAdminApi
    {
        Task<(TokenCryptoData?, GetTokenCryptoDataError?)> GetTokenCryptoDataAsync(Guid audience, CancellationToken cancellationToken = default);

        Task<(TokenCryptoData?,GetOrCreateTokenCryptoDataError?)> GetOrCreateTokenCryptoDataAsync(Guid audience,
            CancellationToken cancellationToken = default);
    }
}