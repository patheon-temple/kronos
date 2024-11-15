using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public interface IHermesAdminApi
    {
        Task<TokenCryptoData> CreateTokenCryptoDataAsync(Guid audience, CancellationToken cancellationToken = default);

        Task<TokenCryptoData?> GetTokenCryptoDataAsync(Guid audience, CancellationToken cancellationToken = default);

        Task<TokenCryptoData> GetOrCreateTokenCryptoDataAsync(Guid audience,
            CancellationToken cancellationToken = default);
    }
}