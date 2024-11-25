using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public enum CreateTokenSetError
    {
        NonExistingAudience,
        ServiceCredentialsInvalid,
        FailedToCreateToken,
        InvalidCredentials
    }

    public interface IHermesApi
    {
        Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetAsync(CreateTokenSetArgs args,
            CancellationToken cancellationToken);
    }
}