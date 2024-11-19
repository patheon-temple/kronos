using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(
    IAthenaApi athenaApi,
    IHermesAdminApi hermesAdminApi
) : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string password, string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var identity = await athenaApi.GetValidatedUserByDeviceIdAsync(deviceId, password, cancellationToken);
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

        var identity = await athenaApi.GetValidatedUserByUsernameAsync(username, password, cancellationToken);

        if (identity is null)
            throw new UserNotFoundExistException
            {
                Data = { { "username", username } }
            };
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
        var service = await athenaApi.GetValidatedServiceAsync(serviceId, authorizationCode, cancellationToken);
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