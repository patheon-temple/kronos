using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public interface IAthenaApi
    {
        Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
            CancellationToken cancellationToken = default);

        Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);

        Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default);

        Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password,
            CancellationToken stoppingToken = default);

        Task<PantheonIdentity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> VerifyPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);
    }
}