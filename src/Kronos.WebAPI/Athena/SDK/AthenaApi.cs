using Athena.SDK;
using Athena.SDK.Models;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaApi(
    IDbContextFactory<AthenaDbContext> contextFactory,
    IOptionsSnapshot<AthenaConfiguration> optionsSnapshot) : IAthenaApi
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

        await IsUsernameAndPasswordValidOrThrowAsync(username, password, cancellationToken);

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

    private async Task IsUsernameAndPasswordValidOrThrowAsync(string username, string password, CancellationToken cancellationToken)
    {
        if (await DoesUsernameExistAsync(username, cancellationToken))
            throw new ValidationException("Username already exists.");

        var passwordValidator = Passwords.CreateValidator();
        var validationResult = await passwordValidator.ValidateAsync(new UserCredentialsValidationParams
        {
            Username = username,
            Password = password
        }, cancellationToken);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
    }

    public async Task<PantheonIdentity?> GetUserByUsernameAsync(string username,
        CancellationToken cancellationToken = default)
    {
        username = username.ToLower();

        if (username.Equals(optionsSnapshot.Value.SuperuserUsername))
            return new PantheonIdentity
            {
                DeviceId = null,
                Id = optionsSnapshot.Value.SuperuserId
            };
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<bool> VerifyPasswordAsync(Guid userId, string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        if (userId.Equals(optionsSnapshot.Value.SuperuserId))
            return optionsSnapshot.Value.SuperuserPassword.Equals(password);

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var passwordHash = await db.UserAccounts.Where(x => x.UserId == userId).Select(x => x.PasswordHash)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (passwordHash is null) return false;

        return passwordHash.Length > 0 && Passwords.VerifyHashedPassword(passwordHash, password);
    }

    public async Task<PantheonIdentity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }
}