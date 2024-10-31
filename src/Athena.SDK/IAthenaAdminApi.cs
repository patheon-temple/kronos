using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public interface IAthenaAdminApi
    {
        Task<PantheonIdentity> CreateUserAsync(string? deviceId, string? username, string? password, CancellationToken cancellationToken = default);
    }
}