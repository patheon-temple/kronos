using Kronos.WebAPI.Athena.Domain;
using Microsoft.EntityFrameworkCore.Internal;

namespace Kronos.WebAPI.Athena.SDK;

public interface IAthenaApi
{
    Task<PantheonIdentity> CreateIdentityByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default);
}