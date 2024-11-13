using System.Security;
using Athena.SDK;
using Athena.SDK.Models;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;
using Kronos.WebAPI.Hermes.WebApi;
using Kronos.WebAPI.Hermes.WebApi.Interop.Shared;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(IAthenaApi athenaApi, IAthenaAdminApi athenaAdminApi, TokenService tokenService, IOptions<HermesConfiguration> options)
    : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, string[] requestedScopes,
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

        return CreateTokenSet(requestedScopes, identity);
    }

    private static string[] FilterScopes(string[] requestedScopes, string[] availableScopes)
    {
        var validScopes = requestedScopes.Where(availableScopes.Contains).ToArray();
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

    public async Task<TokenSet> CreateTokenSetForServiceAsync(Guid serviceId, byte[] secret, string[] requestedScopes,
        CancellationToken cancellationToken = default)
    {
        var service = await athenaAdminApi.GetServiceAccountByIdAsync(serviceId, cancellationToken);
        if (service is null)
        {
            throw new Exception("Service not found");
        }
        
        if(!service.Secret.SequenceEqual(secret)) throw new SecurityException("Secret mismatch");
        
        var validScopes = FilterScopes(requestedScopes, service.Scopes);

        var accessToken = tokenService.CreateServiceAccessToken(service.Name, serviceId, validScopes);
        return new TokenSet
        {
            AccessToken = accessToken,
            Scopes = validScopes,
            UserId = service.Id.ToString("N"),
            Username = service.Name
        };
    }

    private TokenSet CreateTokenSet(string[] requestedScopes, PantheonIdentity identity)
    {
        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        var accessToken = tokenService.CreateUserAccessToken(identity.Username, identity.Id,
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