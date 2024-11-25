using Hermes.SDK;
using Kronos.WebAPI.Hermes.Mappers;
using Kronos.WebAPI.Hermes.SDK;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Hermes.Services;

public class HermesRepository(
    IDbContextFactory<HermesDbContext> contextFactory
)
{
    public async Task<TokenCryptoData?> GetTokenDataAsync(Guid audience, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = await db.TokenCryptoData.FirstOrDefaultAsync(x => x.EntityId == audience,
            cancellationToken: cancellationToken);
        return data is null ? null : HermesMappers.ToDomain(data);
    }

    public async Task<TokenCryptoData> CreatTokenCryptoDataAsync(byte[] encryptionData, byte[] signingData, Guid audience,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var data = new TokenCryptoDataModel
        {
            EncryptionKey = encryptionData,
            SigningKey = signingData,
            EntityId = audience,
        };
        await db.TokenCryptoData.AddAsync(data, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        
         return HermesMappers.ToDomain(data);

    }
}