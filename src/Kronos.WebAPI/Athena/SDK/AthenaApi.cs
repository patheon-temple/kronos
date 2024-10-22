using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaApi(IDbContextFactory<AthenaDbContext> contextFactory) : IAthenaApi
{
    public async Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var identity = await db.UserAccounts.FirstOrDefaultAsync(
            entity => entity.DeviceId != null && entity.DeviceId.Equals(deviceId), cancellationToken);

        if (identity is not null) return IdentityMappers.ToDomain(identity);

        await db.AddAsync(identity = new UserAccountDataModel
        {
            DeviceId = deviceId
        }, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return IdentityMappers.ToDomain(identity);
    }

    public async Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await db.UserAccounts.FirstOrDefaultAsync(
            x => x.DeviceId != null && x.DeviceId.Equals(deviceId), cancellationToken: cancellationToken);
        return entity is null ? null : IdentityMappers.ToDomain(entity);
    }

    public async Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
    {
        username = username.ToLower();
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.AnyAsync(x => x.Username != null && x.Username.Equals(username),
            cancellationToken);
    }

    public Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password, CancellationToken stoppingToken = default)
    {
        username = username.ToLower();
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
    }
}