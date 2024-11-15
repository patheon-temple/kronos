using System.Security;
using System.Security.Cryptography;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;
using Kronos.WebAPI.Hermes.WebApi.Interop.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(
    IAthenaApi athenaApi,
    IAthenaAdminApi athenaAdminApi,
    IOptions<HermesConfiguration> options,
    IDbContextFactory<HermesDbContext> contextFactory
)
    : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var identity = await athenaApi.GetUserByDeviceIdAsync(deviceId, cancellationToken);
        if (identity is null)
        {
            if (!options.Value.Registration.IsEnabled(CredentialsType.DeviceId))
                throw new SecurityException("Forbidden");

            identity = await athenaApi.CreateUserFromDeviceIdAsync(deviceId, CancellationToken.None);
        }

        var encData = await GetTokenCryptoDataAsyncOrThrow(audience, cancellationToken);

        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = identity.Username,
            Scopes = validScopes,
            Id = identity.Id,
        }, encData, GlobalDefinitions.AccountType.User);

        return new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
        };
    }

    private async Task<TokenCryptoData> GetTokenCryptoDataAsyncOrThrow(Guid audience,
        CancellationToken cancellationToken)
    {
        if (GlobalDefinitions.Jwt.AthenaAudience.Equals(audience))
        {
            return options.Value.Jwt.ToTokenCryptoData();
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.TokenCryptoData.FirstOrDefaultAsync(x => x.EntityId == audience,
            cancellationToken: cancellationToken);

        if (data is not null)
            return new TokenCryptoData
            {
                EncryptionKey = data.EncryptionKey,
                EntityId = data.EntityId,
                SigningKey = data.SigningKey,
            };

        var doesExists = await athenaAdminApi.ServiceAccountExistsAsync(audience, cancellationToken);
        if (!doesExists)
            throw new SecurityException($"No audience with id {audience} found");
        
        var encryptionData = new byte[GlobalDefinitions.Limits.EncryptionKeyMaxSize];
        var signingData = new byte[GlobalDefinitions.Limits.SigningKeyMaxSize];
        
        using var rand = RandomNumberGenerator.Create();
        rand.GetNonZeroBytes(encryptionData);
        rand.GetNonZeroBytes(signingData);
        data = new TokenCryptoDataModel
        {
            EncryptionKey = encryptionData,
            SigningKey = signingData,
            EntityId = audience,
        };
        await db.TokenCryptoData.AddAsync(data, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return new TokenCryptoData
        {
            EncryptionKey = data.EncryptionKey,
            EntityId = data.EntityId,
            SigningKey = data.SigningKey,
        };
    }

    public async Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password,
        string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var identity = await athenaApi.GetUserByUsernameAsync(username, cancellationToken);

        if (identity is null)
            throw new UserNotFoundExistException
            {
                Data = { { "username", username } }
            };

        if (!await athenaApi.VerifyPasswordAsync(identity.Id, password, cancellationToken))
        {
            throw new SecurityException();
        }

        var encData = await GetTokenCryptoDataAsyncOrThrow(audience, cancellationToken);
        if (encData is null) throw new SecurityException($"No audience with id {audience} found");

        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = identity.Username,
            Scopes = validScopes,
            Id = identity.Id,
        }, encData, GlobalDefinitions.AccountType.User);

        return new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
        };
    }

    public async Task<TokenSet> CreateTokenSetForServiceAsync(Guid serviceId, byte[] secret, string[] requestedScopes,
        Guid audience,
        CancellationToken cancellationToken = default)
    {
        var service = await athenaAdminApi.GetServiceAccountByIdAsync(serviceId, cancellationToken);
        if (service is null)
        {
            throw new Exception("Service not found");
        }

        if (!service.AuthorizationCode.SequenceEqual(secret)) throw new SecurityException("Secret mismatch");

        var encData = await GetTokenCryptoDataAsyncOrThrow(audience, cancellationToken);

        var validScopes = FilterScopes(requestedScopes, service.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = service.Id.ToString("N"),
            Scopes = validScopes,
            Id = service.Id,
        }, encData, GlobalDefinitions.AccountType.User);

        return new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = service.Id.ToString("N"),
            Username = service.Id.ToString("N"),
        };
    }

    private static string[] FilterScopes(string[] requestedScopes, string[] availableScopes)
    {
        var validScopes = requestedScopes.Where(availableScopes.Contains).ToArray();
        return validScopes;
    }
}