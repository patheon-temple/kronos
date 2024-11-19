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
    IPasswordService passwordService,
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
        var entity = await db.UserAccounts
            .Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.DeviceId != null && x.DeviceId.Equals(deviceId),
                cancellationToken: cancellationToken);
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
            PasswordHash = passwordService.HashPassword(password)
        };
        db.UserAccounts.Add(entity);

        await db.SaveChangesAsync(cancellationToken);
        return IdentityMappers.ToDomain(entity);
    }

    private async Task IsUsernameAndPasswordValidOrThrowAsync(string username, string password,
        CancellationToken cancellationToken)
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
            return SuperUserIdentity;

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.Equals(username),
                cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<bool> ValidateUserCredentialsAsync(Guid userId, string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var user = await GetUserByIdAsync(userId, cancellationToken);

        return user != null && passwordService.VerifyUserAccountPassword(user, password);
    }

    public async Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var data = await GetUserByUsernameAsync(username, cancellationToken);
        return data is not null && passwordService.VerifyUserAccountPassword(data, password);
    }

    private PantheonIdentity SuperUserIdentity => new()
    {
        DeviceId = null,
        Id = optionsSnapshot.Value.SuperuserId,
        Scopes = [GlobalDefinitions.Scopes.Superuser],
        PasswordHash = optionsSnapshot.Value.SuperuserPasswordEncoded,
        Username = optionsSnapshot.Value.SuperuserUsername
    };

    public async Task<PantheonIdentity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId.Equals(optionsSnapshot.Value.SuperuserId))
        {
            return SuperUserIdentity;
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.ServiceAccounts.Where(x => x.Id == serviceId)
            .Select(x => x.AuthorizationCode)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return data is not null && passwordService.VerifyAuthorizationCode(data, authorizationCode);
    }

    public async Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.UserAccounts.Where(x => x.Id == userId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (data is null)
            throw new NullReferenceException("Not found");

        data.PasswordHash = passwordService.HashPassword(Guid.NewGuid().ToString("D"));
        db.Update(data);
        await db.SaveChangesAsync(cancellationToken);

        return passwordService.DecodePassword(data.PasswordHash);
    }
}