using System.Security.Cryptography;
using System.Text;
using Athena.SDK;
using Athena.SDK.Models;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaAdminApi(IDbContextFactory<AthenaDbContext> contextFactory, IAthenaApi athenaApi)
    : IAthenaAdminApi
{
    public async Task<PantheonIdentity> CreateUserAsync(
        string? deviceId,
        string? username,
        string? password,
        string[] scopes,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deviceId) && string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(deviceId),
                $"{nameof(deviceId)} and {nameof(username)} cannot be null or empty.");

        if (!string.IsNullOrWhiteSpace(username))
        {
            await IsUsernameAndPasswordValidOrThrowAsync(username, password, cancellationToken);
        }

        scopes = scopes.Where(x => !x.Equals(GlobalDefinitions.Scopes.Superuser)).ToArray();

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var scopeEntities = await db.UserScopes
            .Where(x => scopes.Contains(x.Id))
            .ToArrayAsync(cancellationToken: cancellationToken);

        var identity = new UserAccountDataModel
        {
            DeviceId = deviceId,
            PasswordHash = string.IsNullOrWhiteSpace(password) ? null : Passwords.HashPassword(password),
            Username = username?.ToLower(),
            Scopes = scopeEntities
        };
        db.UserAccounts.Add(identity);
        await db.SaveChangesAsync(cancellationToken);

        return IdentityMappers.ToDomain(identity);
    }

    public async Task<PantheonIdentity?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var data = await db.UserAccounts.Include(x => x.Scopes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    private async Task IsUsernameAndPasswordValidOrThrowAsync(string username, string? password,
        CancellationToken cancellationToken)
    {
        if (await athenaApi.DoesUsernameExistAsync(username, cancellationToken))
            throw new ValidationException("Username already exists.");

        var passwordValidator = Passwords.CreateValidator();
        var validationResult = await passwordValidator.ValidateAsync(new UserCredentialsValidationParams
        {
            Username = username,
            Password = password
        }, cancellationToken);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
    }

    public async Task<PantheonService> CreateServiceAccountAsync(string serviceName, string[] requiredScopes,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        requiredScopes = requiredScopes.Where(x => !x.Equals(GlobalDefinitions.Scopes.Superuser)).ToArray();

        var scopes = requiredScopes.Length == 0
            ? []
            : await db.ServiceScopes.Where(x => requiredScopes.Contains(x.Id))
                .ToArrayAsync(cancellationToken: cancellationToken);

        var salt = new byte[512];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var data = await db.ServiceAccounts.AddAsync(
            new ServiceAccountDataModel
            {
                Id = Guid.NewGuid(),
                Name = serviceName,
                AuthorizationCode = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),
                Scopes = scopes,
            },
            cancellationToken: cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return ServiceMappers.ToDomain(data.Entity);
    }


    public async Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var data = await db.ServiceAccounts.FirstOrDefaultAsync(x => x.Id == serviceId,
            cancellationToken: cancellationToken);

        return data == null ? null : ServiceMappers.ToDomain(data);
    }

    public async Task<bool> ServiceAccountExistsAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        return await db.ServiceAccounts.AnyAsync(x => x.Id == serviceId, cancellationToken: cancellationToken);
    }
}