using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK.Models;

namespace Athena.SDK
{
    public interface IAthenaAdminApi
    {
        Task<PantheonIdentity> CreateUserAsync(
            string? deviceId,
            string? username,
            string? password,
            string[] scopes,
            CancellationToken cancellationToken = default);

        Task<PantheonIdentity?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        Task<PantheonService> CreateServiceAccountAsync(string serviceName, string[] requiredScopes,
            CancellationToken cancellationToken = default);

        Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId,
            CancellationToken cancellationToken = default);
    }
}