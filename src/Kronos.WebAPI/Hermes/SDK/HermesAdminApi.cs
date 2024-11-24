using System.Security;
using System.Security.Cryptography;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Mappers;
using Kronos.WebAPI.Hermes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesAdminApi(
    IOptions<HermesConfiguration> options,
    IAthenaAdminApi athenaAdminApi,
    IDbContextFactory<HermesDbContext> contextFactory) : IHermesAdminApi
{
    public async Task<TokenCryptoData> CreateTokenCryptoDataAsync(Guid audience,
        CancellationToken cancellationToken = default)
    {
        if (GlobalDefinitions.Jwt.AthenaAudience.Equals(audience))
        {
            return options.Value.Jwt.ToTokenCryptoData();
        }

        var doesExists = await athenaAdminApi.ServiceAccountExistsAsync(audience, cancellationToken);
        if (!doesExists)
            throw new SecurityException($"No audience with id {audience} found");

        var encryptionData = new byte[GlobalDefinitions.Limits.EncryptionKeyMaxSize];
        var signingData = new byte[GlobalDefinitions.Limits.SigningKeyMaxSize];

        using var rand = RandomNumberGenerator.Create();
        rand.GetNonZeroBytes(encryptionData);
        rand.GetNonZeroBytes(signingData);

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = new TokenCryptoDataModel
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

    public async Task<TokenCryptoData?> GetTokenCryptoDataAsync(Guid audience,
        CancellationToken cancellationToken = default)
    {
        if (GlobalDefinitions.Jwt.AthenaAudience.Equals(audience))
        {
            return options.Value.Jwt.ToTokenCryptoData();
        }

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.TokenCryptoData.FirstOrDefaultAsync(x => x.EntityId == audience,
            cancellationToken: cancellationToken);
        if (data is null)
            return null;
        return new TokenCryptoData
        {
            EncryptionKey = data.EncryptionKey,
            EntityId = data.EntityId,
            SigningKey = data.SigningKey,
        };
    }

    public async Task<Result<GetOrCreateTokenCryptoDataResult, TokenCryptoData?>> GetOrCreateTokenCryptoDataAsync(
        Guid audience,
        CancellationToken cancellationToken = default)

    {
        if (!await athenaAdminApi.ServiceAccountExistsAsync(audience, cancellationToken))
        {
            return new Result<GetOrCreateTokenCryptoDataResult, TokenCryptoData?>(
                GetOrCreateTokenCryptoDataResult.Failure, null);
        }


        var data = await GetTokenCryptoDataAsync(audience, cancellationToken) ??
                   await CreateTokenCryptoDataAsync(audience, cancellationToken);
        return new Result<GetOrCreateTokenCryptoDataResult, TokenCryptoData?>(GetOrCreateTokenCryptoDataResult.Success,
            data);
    }
}