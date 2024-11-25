using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public enum CreateTokenSetError
    {
        NonExistingAudience,
        NonExistingUsername,
        ServiceCredentialsInvalid,
        FailedToCreateToken
    }

    public interface IHermesApi
    {
        Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetAsync(CreateTokenSetArgs args,
            CancellationToken cancellationToken);
    }
}