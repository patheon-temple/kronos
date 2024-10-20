using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaApi(IDbContextFactory<AthenaDbContext> contextFactory) : IAthenaApi
{
    public async Task<PantheonIdentity> CreateIdentityByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var identity = await db.PantheonIdentities.FirstOrDefaultAsync(
            entity => entity.DeviceId != null && entity.DeviceId.Equals(deviceId), cancellationToken);

        if (identity is not null) return IdentityMappers.ToDomain(identity);
        
        await db.AddAsync(identity = new PantheonIdentityDataModel
        {
            DeviceId = deviceId
        }, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return IdentityMappers.ToDomain(identity);
    }
}