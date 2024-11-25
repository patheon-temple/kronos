using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public enum ResetUserPasswordError
    {
        InvalidPasswordFormat
    }

    public enum GetValidatedEntityError
    {
        IdentityNotFound,
        InvalidCredentials
    }
    public interface IAthenaApi
    {
        Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default);

        Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode, CancellationToken cancellationToken = default);
        Task<(string? NewPassword, ResetUserPasswordError? Error)>ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByDeviceIdAsync(string deviceId, string password, CancellationToken cancellationToken = default);
        Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByUsernameAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<(PantheonService?, GetValidatedEntityError?)> GetValidatedServiceAsync(Guid serviceId, string authorizationCode, CancellationToken cancellationToken = default);
    }
}