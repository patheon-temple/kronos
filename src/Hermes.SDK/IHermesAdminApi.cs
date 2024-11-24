using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public enum GetOrCreateTokenCryptoDataResult
    {
        Success = 1,
        Failure = 2
    }
    public interface IHermesAdminApi
    {
        Task<TokenCryptoData> CreateTokenCryptoDataAsync(Guid audience, CancellationToken cancellationToken = default);

        Task<TokenCryptoData?> GetTokenCryptoDataAsync(Guid audience, CancellationToken cancellationToken = default);

        Task<Result<GetOrCreateTokenCryptoDataResult, TokenCryptoData?>> GetOrCreateTokenCryptoDataAsync(Guid audience,
            CancellationToken cancellationToken = default);
    }
}