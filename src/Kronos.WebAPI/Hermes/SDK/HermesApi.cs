using Athena.SDK;
using Athena.SDK.Models;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(
    IAthenaApi athenaApi,
    IHermesAdminApi hermesAdminApi
) : IHermesApi
{
    public async Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetAsync(CreateTokenSetArgs args,
        CancellationToken cancellationToken)
    {
        return args.CredentialsType switch
        {
            CredentialsType.DeviceId => await CreateTokenSetForDeviceAsync(args.DeviceId!,
                args.Password!, args.RequestedScopes.ToArray(), args.Audience, cancellationToken),
            CredentialsType.Password => await CreateTokenSetForUserCredentialsAsync(args.Username!,
                args.Password!, args.RequestedScopes.ToArray(), args.Audience, cancellationToken),
            CredentialsType.AuthorizationCode => await CreateTokenSetForServiceAsync(
                args.ServiceId!.Value,
                args.AuthorizationCode!, args.RequestedScopes, args.Audience,
                cancellationToken),
            CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetForDeviceAsync(string deviceId,
        string password, string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var identity = await athenaApi.GetValidatedUserByDeviceIdAsync(deviceId, password, cancellationToken);
        if (identity is null) throw new NullReferenceException();

        var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
        if (error is not null)
            return (null, CreateTokenSetError.NonExistingAudience);

        var (accessToken, validScopes) = CreateTokenForAudience(requestedScopes, audience, identity, tokenCryptoData);

        return (new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
        }, null);
    }

    private async Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetForUserCredentialsAsync(string username,
        string password,
        string[] requestedScopes, Guid audience,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var identity = await athenaApi.GetValidatedUserByUsernameAsync(username, password, cancellationToken);

        if (identity is null)
            return (null, CreateTokenSetError.NonExistingUsername);

        var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);

        if (error is not null) return (null, CreateTokenSetError.NonExistingAudience);

        var (accessToken, validScopes) =
            CreateTokenForAudience(requestedScopes, audience, identity, tokenCryptoData);

        return (new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity.Id.ToString(),
            Username = identity.Username,
        }, null);
    }

    private static (string AccessToken, string[] ValidScopes) CreateTokenForAudience(string[] requestedScopes,
        Guid audience, PantheonIdentity identity,
        TokenCryptoData? tokenCryptoData)
    {
        var validScopes = FilterScopes(requestedScopes, identity.Scopes);
        return (TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = identity.Username,
            Scopes = validScopes,
            Id = identity.Id,
        }, tokenCryptoData!, GlobalDefinitions.AccountType.User), validScopes);
    }

    private async Task<(TokenSet?, CreateTokenSetError?)> CreateTokenSetForServiceAsync(Guid serviceId,
        string authorizationCode,
        string[] requestedScopes,
        Guid audience,
        CancellationToken cancellationToken = default)
    {
        var service = await athenaApi.GetValidatedServiceAsync(serviceId, authorizationCode, cancellationToken);
        if (service is null) return (null, CreateTokenSetError.ServiceCredentialsInvalid);

        TokenCryptoData data;

        if (serviceId == audience)
        {
            var (tokenCryptoData, error) =
                await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(serviceId, cancellationToken);
            if (error is not null)
                return (null, CreateTokenSetError.FailedToCreateToken);

            data = tokenCryptoData!;
        }
        else
        {
            var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
            if (error is not null)
                return (null, CreateTokenSetError.FailedToCreateToken);
            
            data = tokenCryptoData!;
        }


        var validScopes = FilterScopes(requestedScopes, service.Scopes);
        var accessToken = TokenService.CreateAccessToken(new TokenUserData
        {
            Audience = audience.ToString("N"),
            Username = service.Id.ToString("N"),
            Scopes = validScopes,
            Id = service.Id,
        }, data, GlobalDefinitions.AccountType.Service);

        return (
            new TokenSet
            {
                Scopes = validScopes,
                AccessToken = accessToken,
                UserId = service.Id.ToString("N"),
                Username = service.Id.ToString("N"),
            }, null);
    }

    private static string[] FilterScopes(string[] requestedScopes, string[] availableScopes)
    {
        var validScopes = requestedScopes.Where(availableScopes.Contains).ToArray();
        return validScopes;
    }
}