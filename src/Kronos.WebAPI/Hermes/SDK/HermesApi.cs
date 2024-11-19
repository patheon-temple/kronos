using System.Security;
using System.Security.Cryptography;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(
    IAthenaApi athenaApi,
    IAthenaAdminApi athenaAdminApi,
    IHermesAdminApi hermesAdminApi,
    IOptions<HermesConfiguration> options
) : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string password, string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var isValid = await athenaApi.ValidateDeviceCredentialsAsync(deviceId, password, cancellationToken);
        if (!isValid)
        {
            throw new SecurityException("Forbidden");
        }

        var identity = await athenaApi.GetUserByDeviceIdAsync(deviceId, cancellationToken);
        if (identity is null) throw new NullReferenceException();
        
        var tokenCryptoData = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
        if (tokenCryptoData is null) throw new Exception($"Audience {audience} is invalid");

        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = identity.Username,
            Scopes = validScopes,
            Id = identity.Id,
        }, tokenCryptoData, GlobalDefinitions.AccountType.User);

        return new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
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

        if (!await athenaApi.ValidateUserCredentialsAsync(identity.Id, password, cancellationToken))
        {
            throw new SecurityException();
        }

        var tokenCryptoData = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
        if (tokenCryptoData is null) throw new Exception($"Audience {audience} is invalid");

        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = identity.Username,
            Scopes = validScopes,
            Id = identity.Id,
        }, tokenCryptoData, GlobalDefinitions.AccountType.User);

        return new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
        };
    }

    public async Task<TokenSet> CreateTokenSetForServiceAsync(Guid serviceId, string authorizationCode,
        string[] requestedScopes,
        Guid audience,
        CancellationToken cancellationToken = default)
    {
        var isValid = await athenaApi.ValidateServiceCodeAsync(serviceId, authorizationCode, cancellationToken);
        if (!isValid)
        {
            throw new Exception("Invalid authorization code");
        }

        var service = await athenaAdminApi.GetServiceAccountByIdAsync(serviceId, cancellationToken);
        if (service is null) throw new Exception($"Service not found {serviceId}");

        var tokenCryptoData = serviceId == audience
            ? await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(serviceId, cancellationToken)
            : await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);

        if (tokenCryptoData is null)
        {
            throw new Exception($"Audience {audience} is invalid");
        }

        var validScopes = FilterScopes(requestedScopes, service.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = service.Id.ToString("N"),
            Scopes = validScopes,
            Id = service.Id,
        }, tokenCryptoData, GlobalDefinitions.AccountType.User);

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