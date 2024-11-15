using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public interface IHermesAdminApi
    {
        Task<TokenCryptoData> GetAudienceTokenDataAsync(Guid audienceId, CancellationToken cancellationToken = default);
    }
}