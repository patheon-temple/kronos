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

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var scopeEntities = await db.Scopes
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
            .FirstOrDefaultAsync(x => x.UserId == id, cancellationToken: cancellationToken);

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
}