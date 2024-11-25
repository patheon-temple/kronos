using Athena.SDK;
using Athena.SDK.Models;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Extensions;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(
    IAthenaApi athenaApi,
    IHermesAdminApi hermesAdminApi,
    ILogger<HermesApi> logger) : IHermesApi
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
        var (identity, getValidatedEntityError) =
            await athenaApi.GetValidatedUserByDeviceIdAsync(deviceId, password, cancellationToken);
        if (getValidatedEntityError is not null)
        {
            logger.LogAthenaApiError(nameof(IAthenaApi.GetValidatedUserByDeviceIdAsync), getValidatedEntityError);

            switch (getValidatedEntityError)
            {
                case GetValidatedEntityError.IdentityNotFound:
                case GetValidatedEntityError.InvalidCredentials:
                    return (null, CreateTokenSetError.InvalidCredentials);
                default:
                    throw new OperationErrorException(getValidatedEntityError);
            }
        }

        var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
        if (error is not null)
        {
            logger.LogHermesAdminApiError(nameof(IHermesAdminApi.GetTokenCryptoDataAsync), error);

            switch (error)
            {
                case GetTokenCryptoDataError.NotFound:
                    logger.LogError("HERMES ADMIN - Failed to fetch Token Crypto Data for audience: {AudienceId}",
                        audience);
                    return (null, CreateTokenSetError.NonExistingAudience);
                default:
                    throw new OperationErrorException(error);
            }
        }

        var (accessToken, validScopes) =
            CreateUserTokenForAudience(requestedScopes, audience, identity!, tokenCryptoData);

        return (new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity!.Id.ToString(),
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

        var (identity, getValidatedEntityError) =
            await athenaApi.GetValidatedUserByUsernameAsync(username, password, cancellationToken);

        if (getValidatedEntityError is not null)
        {
            logger.LogAthenaApiError(nameof(IAthenaApi.GetValidatedUserByUsernameAsync), getValidatedEntityError);
            switch (getValidatedEntityError)
            {
                case GetValidatedEntityError.IdentityNotFound:

                case GetValidatedEntityError.InvalidCredentials:
                    return (null, CreateTokenSetError.InvalidCredentials);
                default:
                    throw new OperationErrorException(getValidatedEntityError);
            }
        }

        var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);

        if (error is not null)
        {
            switch (error)
            {
                case GetTokenCryptoDataError.NotFound:
                    return (null, CreateTokenSetError.NonExistingAudience);
                default:
                    throw new OperationErrorException(error);
            }
        }

        var (accessToken, validScopes) =
            CreateUserTokenForAudience(requestedScopes, audience, identity!, tokenCryptoData);

        return (new TokenSet
        {
            Scopes = validScopes,
            AccessToken = accessToken,
            UserId = identity!.Id.ToString(),
            Username = identity.Username,
        }, null);
    }

    private static (string AccessToken, string[] ValidScopes) CreateUserTokenForAudience(string[] requestedScopes,
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
        var (service, getValidatedEntityError) =
            await athenaApi.GetValidatedServiceAsync(serviceId, authorizationCode, cancellationToken);

        if (getValidatedEntityError is not null)
        {
            logger.LogAthenaApiError(nameof(IAthenaApi.GetValidatedServiceAsync), getValidatedEntityError);
            switch (getValidatedEntityError)
            {
                case GetValidatedEntityError.IdentityNotFound:
                case GetValidatedEntityError.InvalidCredentials:
                    return (null, CreateTokenSetError.ServiceCredentialsInvalid);
                default:
                    throw new OperationErrorException(getValidatedEntityError);
            }
        }


        TokenCryptoData data;

        if (serviceId == audience)
        {
            logger.LogInformation("Service ID and Audience are same, trying to create or get crypto data");
            var (tokenCryptoData, error) =
                await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(serviceId, cancellationToken);
            if (error is not null)
            {
                logger.LogHermesAdminApiError(nameof(IHermesAdminApi.GetOrCreateTokenCryptoDataAsync), error);
                return (null, CreateTokenSetError.FailedToCreateToken);
            }

            data = tokenCryptoData!;
        }
        else
        {
            var (tokenCryptoData, error) = await hermesAdminApi.GetTokenCryptoDataAsync(audience, cancellationToken);
            if (error is not null)
            {
                logger.LogHermesAdminApiError(nameof(IHermesAdminApi.GetOrCreateTokenCryptoDataAsync), error);
                return (null, CreateTokenSetError.FailedToCreateToken);
            }

            data = tokenCryptoData!;
        }


        var validScopes = FilterScopes(requestedScopes, service!.Scopes);
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