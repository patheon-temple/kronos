using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public interface IAthenaApi
    {
        Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default);

        Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode, CancellationToken cancellationToken = default);
        Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<PantheonIdentity?> GetValidatedUserByDeviceIdAsync(string deviceId, string password, CancellationToken cancellationToken = default);
        Task<PantheonIdentity?> GetValidatedUserByUsernameAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<PantheonService?> GetValidatedServiceAsync(Guid serviceId, string authorizationCode, CancellationToken cancellationToken = default);
    }
}