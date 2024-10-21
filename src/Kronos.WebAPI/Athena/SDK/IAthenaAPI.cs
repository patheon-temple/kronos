using Kronos.WebAPI.Athena.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Kronos.WebAPI.Athena.SDK;

public interface IAthenaApi
{
    Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default);

    Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);
}