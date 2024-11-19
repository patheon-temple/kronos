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
        Task<bool> ValidateUserCredentialsAsync(Guid userId, string password, CancellationToken cancellationToken = default);
        Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<PantheonIdentity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode, CancellationToken cancellationToken = default);
        Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> ValidateDeviceCredentialsAsync(string deviceId, string password, CancellationToken cancellationToken = default);
    }
}