using Athena.SDK;
using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.SDK;

public class AthenaRepository(
    IDbContextFactory<AthenaDbContext> contextFactory,
    IOptionsSnapshot<AthenaConfiguration> optionsSnapshot,
    ILogger<AthenaRepository> logger
) : IAthenaRepository
{
    public async Task<UserAccountDataModel?> GetUserAccountByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.DeviceId != null && x.DeviceId.Equals(deviceId),
                cancellationToken: cancellationToken);
    }

    public async Task CreateUserAccountAsync(UserAccountDataModel identity, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        await db.AddAsync(identity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.AnyAsync(x => x.Username != null && x.Username.Equals(username),
            cancellationToken);
    }

    private UserAccountDataModel SuperUser { get; } = new()
    {
        Id = optionsSnapshot.Value.SuperuserId,
        DeviceId = null,
        Username = optionsSnapshot.Value.SuperuserUsername,
        PasswordHash = optionsSnapshot.Value.SuperuserPasswordEncoded,
        Scopes = new List<UserScopeDataModel>
        {
            new()
            {
                Id = GlobalDefinitions.Scopes.Superuser
            }
        }
    };

    public async Task<UserAccountDataModel?> GetUserAccountByUsernameAsync(string username,
        CancellationToken cancellationToken)
    {
        username = username.ToLower();
        logger.LogWarning("Username: {Username} == {Other}", optionsSnapshot.Value.SuperuserUsername, username);
        if (username.Equals(optionsSnapshot.Value.SuperuserUsername))
            return SuperUser;

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);
    }

    public async Task<UserAccountDataModel?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id.Equals(optionsSnapshot.Value.SuperuserId))
        {
            return SuperUser;
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == id,
                cancellationToken: cancellationToken);
    }

    public async Task UpdateUserAsync(UserAccountDataModel data, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        db.Update(data);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ServiceAccountDataModel?> GetServiceAccountAsync(Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.ServiceAccounts
            .Include(x => x.Scopes)
            .Where(x => x.Id == serviceId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}