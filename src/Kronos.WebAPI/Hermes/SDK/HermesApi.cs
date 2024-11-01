using System.Security;
using Athena.SDK;
using Athena.SDK.Models;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(IAthenaApi athenaApi, TokenService tokenService) : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string[] requestedScopes,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var identity = await athenaApi.GetUserByDeviceIdAsync(deviceId, cancellationToken) ??
                       await athenaApi.CreateUserFromDeviceIdAsync(deviceId, CancellationToken.None);
        return CreateTokenSet(requestedScopes, identity);
    }

    private static string[] FilterScopes(string[] requestedScopes, PantheonIdentity identity)
    {
        var validScopes = requestedScopes.Where(identity.Scopes.Contains).ToArray();
        return validScopes;
    }

    public async Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password,
        string[] requestedScopes,
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

        return CreateTokenSet(requestedScopes, identity);
    }

    private TokenSet CreateTokenSet(string[] requestedScopes, PantheonIdentity identity)
    {
        var validScopes = FilterScopes(requestedScopes, identity);
        var accessToken = tokenService.CreateAccessToken(identity.Username, identity.Id,
            validScopes);

        return new TokenSet
        {
            AccessToken = accessToken,
            Scopes = validScopes,
            UserId = identity.Id.ToString("N"),
            Username = identity.Username
        };
    }
}