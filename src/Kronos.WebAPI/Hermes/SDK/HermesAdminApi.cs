using System.Security.Cryptography;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesAdminApi(
    IOptions<HermesConfiguration> options,
    IAthenaAdminApi athenaAdminApi,
    HermesRepository repository,
    ILogger<HermesAdminApi> logger) : IHermesAdminApi
{
    private async Task<(TokenCryptoData?, CreateTokenCryptoDataError?)> CreateTokenCryptoDataAsync(
        Guid audience,
        CancellationToken cancellationToken = default)
    {
        if (GlobalDefinitions.Jwt.IsAthenaAudience(audience))
        {
            logger.LogInformation("Audience is Athena Api, returning token crypto data");
            return (options.Value.Jwt.ToTokenCryptoData(), null);
        }

        var doesExists = await athenaAdminApi.ServiceAccountExistsAsync(audience, cancellationToken);
        if (!doesExists)
        {
            logger.LogError("ATHENA - Service account for audience {AudienceId} doesn't exists", audience);
            return (null, CreateTokenCryptoDataError.NonExistingAudience);
        }

        var encryptionData = new byte[GlobalDefinitions.Limits.EncryptionKeyMaxSize];
        var signingData = new byte[GlobalDefinitions.Limits.SigningKeyMaxSize];

        using var rand = RandomNumberGenerator.Create();
        rand.GetNonZeroBytes(encryptionData);
        rand.GetNonZeroBytes(signingData);

        var data = await repository.CreatTokenCryptoDataAsync(encryptionData, signingData, audience, cancellationToken);

        return (new TokenCryptoData
            {
                EncryptionKey = data.EncryptionKey,
                EntityId = data.EntityId,
                SigningKey = data.SigningKey,
            }, null);
    }

    public async Task<(TokenCryptoData?, GetTokenCryptoDataError?)> GetTokenCryptoDataAsync(Guid audience,
        CancellationToken cancellationToken = default)
    {
        if (GlobalDefinitions.Jwt.AthenaAudience.Equals(audience))
        {
            logger.LogInformation("Audience is Athena Api, returning token crypto data");
            return (options.Value.Jwt.ToTokenCryptoData(), null);
        }

        var data = await repository.GetTokenDataAsync(audience, cancellationToken);

        if (data is null)
            return (null, GetTokenCryptoDataError.NotFound);

        return (new TokenCryptoData
            {
                EncryptionKey = data.EncryptionKey,
                EntityId = data.EntityId,
                SigningKey = data.SigningKey,
            }, null);
    }

    public async Task<(TokenCryptoData?, GetOrCreateTokenCryptoDataError?)> GetOrCreateTokenCryptoDataAsync(
        Guid audience,
        CancellationToken cancellationToken = default)

    {
        if (!await athenaAdminApi.ServiceAccountExistsAsync(audience, cancellationToken))
        {
            logger.LogError("ATHENA ADMIN - Service account for audience {AudienceId} doesn't exists", audience);

            return (null, GetOrCreateTokenCryptoDataError.ServiceAccountNotFound);
        }

        var (data, getTokenError) = await GetTokenCryptoDataAsync(audience, cancellationToken);

        if (getTokenError is null) return (data, null);

        return getTokenError switch
        {
            GetTokenCryptoDataError.NotFound => await CreateInternalAsync(),
            _ => throw new OperationErrorException(getTokenError)
        };

        async Task<(TokenCryptoData?, GetOrCreateTokenCryptoDataError?)> CreateInternalAsync()
        {
            var (tokenCryptoData, error) = await CreateTokenCryptoDataAsync(audience, cancellationToken);
            
            if(error is null) return (tokenCryptoData, null);
            logger.LogError("HERMES ADMIN - Token Crypto Data creation failed for audience: {AudienceId}", audience);

            return (tokenCryptoData, GetOrCreateTokenCryptoDataError.UnableToCreateToken);
        }
    }
}