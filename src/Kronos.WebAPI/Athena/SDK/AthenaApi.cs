using Athena.SDK;
using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Data;
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

    public async Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        username = username.ToLower();
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = new UserAccountDataModel
        {
            DeviceId = null,
            Username = username,
            PasswordHash = Passwords.HashPassword(password)
        };
        db.UserAccounts.Add(entity);

        await db.SaveChangesAsync(cancellationToken);
        return IdentityMappers.ToDomain(entity);
    }

    public async Task<PantheonIdentity?> GetUserByUsernameAsync(string username,
        CancellationToken cancellationToken = default)
    {
        username = username.ToLower();
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public Task<bool> VerifyPasswordAsync(byte[] passwordHash, string password, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return Task.FromResult(passwordHash.Length > 0 && Passwords.VerifyHashedPassword(passwordHash, password));
    }
}